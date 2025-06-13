using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alexa.NET.SmartHome.Utilities;
using Microsoft.Extensions.Logging;

namespace Alexa.NET.Skills.Monoprice.Service;

public class MonopriceService : IDisposable
{
    private readonly TcpPortConnection[] _conns;
    private readonly ILogger<MonopriceService>? _logger;

    // Command queuing infrastructure
    private readonly ConcurrentQueue<QueuedCommand> _commandQueue = new();
    private readonly SemaphoreSlim _queueProcessor = new(1, 1);
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly Task _queueProcessorTask;
    private volatile bool _disposed = false;

    // Command execution timing
    private const int COMMAND_DELAY_MS = 100; // Delay between serial commands
    private const int COMMAND_TIMEOUT_MS = 5000; // Timeout for individual commands

    public MonopriceService(string ipAddress, ILogger<MonopriceService>? logger = null, params int[] tcpPorts)
    {
        _logger = logger;
        _conns = new TcpPortConnection[tcpPorts.Length];

        for (var i = 0; i < tcpPorts.Length; i++)
        {
            _conns[i] = new TcpPortConnection(ipAddress, tcpPorts[i]);
            _conns[i].OpenConnection();
        }

        // Start the background queue processor
        _queueProcessorTask = Task.Run(ProcessCommandQueue);

        _logger?.LogInformation($"MonopriceService initialized with {tcpPorts.Length} controllers");
    }

    public virtual void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        _logger?.LogInformation("MonopriceService disposing...");

        // Signal cancellation and wait for queue processor to finish
        _cancellationTokenSource.Cancel();

        try
        {
            _queueProcessorTask?.Wait(TimeSpan.FromSeconds(5));
        }
        catch (AggregateException ex) when (ex.InnerExceptions.All(e => e is OperationCanceledException))
        {
            // Expected when cancelling
        }

        // Dispose connections
        foreach (var conn in _conns)
            conn?.Dispose();

        _cancellationTokenSource?.Dispose();
        _queueProcessor?.Dispose();
    }

    #region Public API Methods (unchanged interface)

    public List<ZoneStatus> GetStatus()
    {
        var result = new List<ZoneStatus>();

        foreach (var conn in _conns)
        {
            var res = conn.WriteData("?10", 6);
            var lines = res.Split(new[] { "\r\n" }, StringSplitOptions.None);
            result.AddRange(lines.Skip(1).Take(6).Select(line => new ZoneStatus(line)));
        }

        // Clean up zone names
        for (var i = 0; i < result.Count; i++)
            result[i].Name = $"Zone{i + 1}";

        return result;
    }

    public void SetPowerOn(string zone) => SetPowerOn(ParseZone(zone));
    public void SetPowerOn(params int[] zones) => QueueCommands(zones, (zone, conn) => $"<1{GetZoneForController(zone)}PR01");

    public void SetPowerOff(string zone) => SetPowerOff(ParseZone(zone));
    public void SetPowerOff(params int[] zones) => QueueCommands(zones, (zone, conn) => $"<1{GetZoneForController(zone)}PR00");

    public void SetMute(bool mute, string zone) => SetMute(mute, ParseZone(zone));
    public void SetMute(bool mute, params int[] zones) => QueueCommands(zones, (zone, conn) => $"<1{GetZoneForController(zone)}MU0{(mute ? "1" : "0")}");

    public void SetVolume(int volume, string zone) => SetVolume(volume, ParseZone(zone));
    public void SetVolume(int volume, params int[] zones)
    {
        if (volume < 0 || volume > 38)
            throw new ArgumentOutOfRangeException(nameof(volume), "The volume must be between 0 and 38.");

        var volumeStr = volume.ToString("D2");
        QueueCommands(zones, (zone, conn) => $"<1{GetZoneForController(zone)}VO{volumeStr}");
    }

    public void SetSource(int source, string zone) => SetSource(source, ParseZone(zone));
    public void SetSource(int source, params int[] zones)
    {
        if (source < 1 || source > 6)
            throw new ArgumentOutOfRangeException(nameof(source), "The source must be between 1 and 6.");

        var sourceStr = source.ToString("D2");
        QueueCommands(zones, (zone, conn) => $"<1{GetZoneForController(zone)}CH{sourceStr}");
    }

    public void SetBalance(int balance, string zone) => SetBalance(balance, ParseZone(zone));
    public void SetBalance(int balance, params int[] zones)
    {
        if (balance < 0 || balance > 20)
            throw new ArgumentOutOfRangeException(nameof(balance), "The balance must be between 0 and 20. (0-9 is Left, 10 is Center, and 11-20 is Right)");

        var balanceStr = balance.ToString("D2");
        QueueCommands(zones, (zone, conn) => $"<1{GetZoneForController(zone)}BL{balanceStr}");
    }

    public void SetBass(int bass, string zone) => SetBass(bass, ParseZone(zone));
    public void SetBass(int bass, params int[] zones)
    {
        if (bass < 0 || bass > 14)
            throw new ArgumentOutOfRangeException(nameof(bass), "The bass must be between 0 and 14. (0-6 is Decrease, 7 is Flat, and 8-14 is Increase)");

        var bassStr = bass.ToString("D2");
        QueueCommands(zones, (zone, conn) => $"<1{GetZoneForController(zone)}BS{bassStr}");
    }

    public void SetTreble(int treble, string zone) => SetTreble(treble, ParseZone(zone));
    public void SetTreble(int treble, params int[] zones)
    {
        if (treble < 0 || treble > 14)
            throw new ArgumentOutOfRangeException(nameof(treble), "The treble must be between 0 and 14. (0-6 is Decrease, 7 is Flat, and 8-14 is Increase)");

        var trebleStr = treble.ToString("D2");
        QueueCommands(zones, (zone, conn) => $"<1{GetZoneForController(zone)}TR{trebleStr}");
    }

    #endregion

    #region Command Queuing Infrastructure

    private void QueueCommands(int[] zones, Func<int, TcpPortConnection, string> commandBuilder)
    {
        foreach (var zone in zones)
        {
            var controllerIndex = GetControllerByZone(zone);
            var command = commandBuilder(zone, _conns[controllerIndex]);

            var queuedCommand = new QueuedCommand
            {
                Command = command,
                ControllerIndex = controllerIndex,
                Zone = zone,
                QueuedAt = DateTime.UtcNow
            };

            _commandQueue.Enqueue(queuedCommand);
            _logger?.LogDebug($"Queued command for Zone {zone}: {command}");
        }

        // Signal the processor that new commands are available
        _queueProcessor.Release();
    }

    private async Task ProcessCommandQueue()
    {
        _logger?.LogInformation("Command queue processor started");

        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                // Wait for commands to be available
                await _queueProcessor.WaitAsync(_cancellationTokenSource.Token);

                // Process all available commands
                while (_commandQueue.TryDequeue(out var queuedCommand) && !_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await ExecuteCommand(queuedCommand);

                    // Add delay between commands to prevent serial communication issues
                    if (!_commandQueue.IsEmpty)
                    {
                        await Task.Delay(COMMAND_DELAY_MS, _cancellationTokenSource.Token);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when shutting down
                break;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in command queue processor");
                // Continue processing other commands
            }
        }

        _logger?.LogInformation("Command queue processor stopped");
    }

    private async Task ExecuteCommand(QueuedCommand queuedCommand)
    {
        try
        {
            var age = DateTime.UtcNow - queuedCommand.QueuedAt;
            if (age.TotalMilliseconds > COMMAND_TIMEOUT_MS)
            {
                _logger?.LogWarning($"Dropping stale command for Zone {queuedCommand.Zone}: {queuedCommand.Command} (age: {age.TotalMilliseconds:F0}ms)");
                return;
            }

            _logger?.LogDebug($"Executing command for Zone {queuedCommand.Zone}: {queuedCommand.Command}");

            // Execute the command synchronously (your TcpPortConnection is synchronous)
            await Task.Run(() =>
            {
                _conns[queuedCommand.ControllerIndex].WriteData(queuedCommand.Command);
            });

            _logger?.LogDebug($"Successfully executed command for Zone {queuedCommand.Zone}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Failed to execute command for Zone {queuedCommand.Zone}: {queuedCommand.Command}");
        }
    }

    #endregion

    #region Private Helper Methods (unchanged)

    private int ParseZone(string zone)
    {
        return int.Parse(zone.Substring(4));
    }

    private int GetControllerByZone(int zone)
    {
        return (zone - 1) / 6;
    }

    private int GetZoneForController(int zone)
    {
        return ((zone - 1) % 6) + 1;
    }

    #endregion

    #region Internal Classes

    private class QueuedCommand
    {
        public string Command { get; set; } = string.Empty;
        public int ControllerIndex { get; set; }
        public int Zone { get; set; }
        public DateTime QueuedAt { get; set; }
    }

    #endregion
}