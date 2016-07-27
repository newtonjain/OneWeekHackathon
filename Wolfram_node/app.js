// ----------------------------------------------------------------------------
// Copyright (c) 2015 Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

// This is a base-level Azure Mobile App SDK.
var express = require('express')

// Set up a standard Express app
var app = express();
 app.listen(process.env.PORT || 3000);   // Listen for requests



// If you are producing a combined Web + Mobile app, then you should handle
// anything like logging, registering middleware, etc. here

// Configuration of the Azure Mobile Apps can be done via an object, the
// environment or an auxiliary file.  For more information, see
// // http://azure.github.io/azure-mobile-apps-node/global.html#configuration
// var mobile = azureMobileApps({
//     // Explicitly enable the Azure Mobile Apps home page
//     homePage: true
// });

// // Import the files from the tables directory to configure the /tables endpoint
// mobile.tables.import('./tables');

// // Import the files from the api directory to configure the /api endpoint
// mobile.api.import('./api');

// // Initialize the database before listening for incoming requests
// // The tables.initialize() method does the initialization asynchronously
// // and returns a Promise.
// mobile.tables.initialize()
//     .then(function () {
//         app.use(mobile);    // Register the Azure Mobile Apps middleware
//         app.listen(process.env.PORT || 3000);   // Listen for requests
//     });
    
  
var Client = require('node-wolfram');
var Wolfram = new Client('2VLJWY-792UJTKU4T');

app.get('/hello', function(req, res) {
    var answer={};
    console.log('here is request', req.query.thebigquestion);
    var question = req.query.thebigquestion;
  Wolfram.query(question, function(err, result) {
    if(err) {
        console.log(err);
    }
    else
    {
        for(var a=0; a<result.queryresult.pod.length; a++)
        {
            var pod = result.queryresult.pod[a];
            var title = pod.$.title;
            console.log(title,": ");
            
            for(var b=0; b<pod.subpod.length; b++)
            {
                var subpod = pod.subpod[b];
                for(var c=0; c<subpod.plaintext.length; c++)
                {
                    var text = subpod.plaintext[c];
                    console.log('\t', text);
   
                    answer[title]=text;
                    
                }
            }
        }
    }
    console.log('////', answer);
  res.send(answer);
});

});

