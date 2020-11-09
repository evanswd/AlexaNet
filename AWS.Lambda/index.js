'use strict';
const https = require('https');

const httpsAgent = https.Agent({
  keepAlive: true,
  keepAliveMsecs: 300000
});

exports.handler = async (event, context, callback) => {
    let resultString = '';
    let body = JSON.stringify(event);
    
    const response = await new Promise((resolve, reject) => {
        
        const options = {
            hostname: 'alexanet.libertyfoxtech.com',
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Content-Length': body.length
            },
            path: '/Alexa/MonopriceSkill',
            agent: httpsAgent,
            timeout: 30 /* maximum time for api gateway invoked Lambda */
        };
        
        const req = https.request(options, res => {
            //console.log(`statusCode: ${res.statusCode}`)
            res.on('data', chunk => {
                resultString += chunk;
            });
            res.on('end', () => {
                console.log("Response Data: " + resultString);
                resolve(JSON.parse(resultString));
            });
        });
        
        req.on('error', error => {
            console.log("Error: " + JSON.stringify(error));
            //Danger... infinite loop...
            console.log("Failed but trying again!!");
            resolve(exports.handler(event, context, callback));
            /*reject({
                statusCode: 500,
                body: 'Something went wrong!'
            });*/
        });
        
        console.log("Request Data: " + body);
        req.write(body);
        req.end();
    });
    
    return response;
};
