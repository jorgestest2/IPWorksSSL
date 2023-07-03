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

main().catch(e => console.log(e))

async function main () {
  console.log('*****************************************************************\n')
  console.log('* This demo shows how to set up an echo server using WSServer.  *\n')
  console.log('*****************************************************************\n')

  const wsserver = new ipworksssl.wsserver()
  const certmgr1 = new ipworksssl.certmgr()

  const question1 = () => {
    return new Promise((resolve, reject) => {
      rl.question('Local Port: ', (port) => {
        resolve(port)
      })
    })
  }

  const question2 = () => {
    return new Promise((resolve, reject) => {
      rl.question('Path to certificate: ', (path) => {
        resolve(path)
      })
    })
  }

  const question3 = () => {
    return new Promise((resolve, reject) => {
      rl.question('Certificate password: ', (password) => {
        resolve(password)
      })
    })
  }

  wsserver.on('Connected', (e) => {
    console.log(wsserver.getConnections().item(e.connectionId).getRemoteHost() + ' connected.')
  }).on('Disconnected', (e) => {
    console.log('Remote host disconnected: ' + e.description)
  }).on('DataIn', async e => {
    console.log(`Received a message from ${wsserver.getConnections().item(e.connectionId).getRemoteHost()} and the message is: '${e.text}'`)
    console.log('Echoing message back....')
    await wsserver.sendText(e.connectionId, e.text).catch(e => console.log(e))
  }).on('SSLClientAuthentication', e => {
    e.accept = true
  })

  wsserver.setLocalHost('localhost')

  wsserver.setLocalPort(await question1())

  let path = await question2();
  let password = await question3();

  const cert = new ipworksssl.Certificate(ipworksssl.CertStoreTypes.cstPFXFile, path, password, "*")

  wsserver.setSSLCert(cert)

  await wsserver.startListening()
  console.log('Listening... press Ctrl-C to shutdown')

  while (true) {
    await wsserver.doEvents()
  }
}

function prompt(promptName, label, punctuation, defaultVal)
{
  lastPrompt = promptName;
  lastDefault = defaultVal;
  process.stdout.write(`${label} [${defaultVal}] ${punctuation} `);
}
