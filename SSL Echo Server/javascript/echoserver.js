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
  if (argv.length !== 5) {
    console.log("Usage: node echoserver.js port filename password");
    console.log("  port	      the TCP port in the local host where the component listens");
    console.log("  filename   the path to the file containing certificates and optional private keys");
    console.log("  password   the password for the certificate store file. If the provided test file is used (test.pfx), set the password to \"test\"");
    console.log("Example: node echoserver.js 777 ../../test.pfx test");
    return;
  }

  const sslserver = new ipworksssl.sslserver();

  sslserver.on("SSLClientAuthentication", function (e) {
    e.accept = true;
  })
  .on("Disconnected", function (e) {
    console.log("\nDisconnected " + e.description + " from " + e.connectionId);
  })
  .on("Connected", function (e) {
    let connections = sslserver.getConnections();
    let connection = connections.get(e.connectionId);
    console.log(connection.getRemoteHost(e.connectionId) + " has connected.");
  })
  .on("DataIn", function (e) {
    let connections = sslserver.getConnections();
    let connection = connections.get(e.connectionId);
    console.log("Echoing '" + e.text + "' to client " + connection.getRemoteHost() + ".");
    sslserver.sendText(e.connectionId, e.text);
  });

  const cert = new ipworksssl.Certificate(ipworksssl.CertStoreTypes.cstAuto, argv[3], argv[4], "*");
  
  sslserver.setSSLCert(cert);

  sslserver.setLocalPort(argv[2]);

  await sslserver.startListening().catch((err) => {
    console.log(err);
  });

  console.log("Started Listening.");

  while (true) {
    await sslserver.doEvents(function (err) {
      if (err) {
        console.log(err);
        return;
      }
    });
  }
}


function prompt(promptName, label, punctuation, defaultVal)
{
  lastPrompt = promptName;
  lastDefault = defaultVal;
  process.stdout.write(`${label} [${defaultVal}] ${punctuation} `);
}
