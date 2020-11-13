﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Alexa.NET.SmartHome.Domain.Constants
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ErrorTypes
    {
        ALREADY_IN_OPERATION,
        BRIDGE_UNREACHABLE,
        CLOUD_CONTROL_DISABLED,
        ENDPOINT_BUSY,
        ENDPOINT_LOW_POWER,
        ENDPOINT_UNREACHABLE,
        EXPIRED_AUTHORIZATION_CREDENTIAL,
        FIRMWARE_OUT_OF_DATE,
        HARDWARE_MALFUNCTION,
        INSUFFICIENT_PERMISSIONS,
        INTERNAL_ERROR,
        INVALID_AUTHORIZATION_CREDENTIAL,
        INVALID_DIRECTIVE,
        INVALID_VALUE,
        NO_SUCH_ENDPOINT,
        NOT_CALIBRATED,
        NOT_SUPPORTED_IN_CURRENT_MODE,
        NOT_IN_OPERATION,
        POWER_LEVEL_NOT_SUPPORTED,
        RATE_LIMIT_EXCEEDED,
        TEMPERATURE_VALUE_OUT_OF_RANGE,
        TOO_MANY_FAILED_ATTEMPTS,
        VALUE_OUT_OF_RANGE
    }
}
