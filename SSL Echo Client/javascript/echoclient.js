/*
 * IPWorks SSL 2022 JavaScript Edition - Sample Project
 *
 * This sample project demonstrates the usage of IPWorks SSL in a 
 * simple, straightforward way. This is not intended to be a complete 
 * application. Error handling and other checks are simplified for clarity.
 *
 * Copyright (c) 2023 /n software inc. www.nsoftware.com
 */
 
const readline = require("readline");
const ipworksssl = require("@nsoftware/ipworksssl");

if(!ipworksssl) {
  console.error("Cannot find ipworksssl.");
  process.exit(1);
}
let rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout
});

main();
async function main() {
  const argv = process.argv;
  if (argv.length != 6) {
    console.log("Usage: node echoclient.js -s server -p port");
    console.log("Options: ");
    console.log("  -s    the address of the remote host.");
    console.log("  -p    the TCP port of the remote host");
    console.log("Example: node echoclient.js -s localhost -p 777");
    process.exit();
  }

  const sslclient = new ipworksssl.sslclient();
  sslclient.config("AcceptAnyServerCert=true");
  let server, port;

  for (i = 0; i < argv.length; i++) {
    if (argv[i].startsWith("-")) {
      if (argv[i] === "-s") { server = argv[i + 1]; }
      if (argv[i] === "-p") { port = argv[i + 1]; }
    }
  }

  function clientprompt() {
    process.stdout.write(' ');
  }

  sslclient.on('SSLServerAuthentication', function (e) {
    e.accept = true;
  })
    .on('Connected', function (e) {
      console.log(sslclient.getRemoteHost() + " has connected.")
    })
    .on('Disconnected', function (e) {
      console.log("Disconnected " + e.description + " from " + sslclient.getRemoteHost() + ".");
    });

  await sslclient.connectTo(server, parseInt(port)).catch((err) => {
    console.log("Error: " + err.message);
    process.exit();
  });

  clientprompt();

  await sslclient.doEvents().catch((err) => {
    console.log("Error: " + err.message);
    process.exit();
  });

  if (sslclient.isConnected()) {
    console.log("> Press 1 to input data \n > Press 2 to quit")
    rl.prompt();
    rl.on('line', command => {
      
      if ("1" === command) {
        rl.question("Type data to send: ", data => {
          sslclient.sendText(data);
          
          sslclient.on('DataIn', function (e) {
            console.log("Received '" + e.text + "' from " + sslclient.getRemoteHost());
            clientprompt();
            rl.prompt();
          })

        })

      } else if ("2" === command) {
        rl.close()
      } else {
        console.log("\r\nInvalid input!");
        rl.prompt()
      }
    }).on('close', () => {
      sslclient.disconnect();
    })
  }

}


function prompt(promptName, label, punctuation, defaultVal)
{
  lastPrompt = promptName;
  lastDefault = defaultVal;
  process.stdout.write(`${label} [${defaultVal}] ${punctuation} `);
}
