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

function wsprompt() {
	process.stdout.write('ws> ');
}

async function main() {

	const wsclient = new ipworksssl.wsclient();

	rl.on('line', (line) => {
		if (line.toLowerCase() === 'quit') {
			console.log('Quitting');
			process.exit();
		}
		else {

			wsclient.send(Buffer.from(line), function (err) {
				if (err) {
					console.log(err);
					process.exit(2);
				}

				wsprompt();
			});
		}
	});


	wsclient.on('DataIn', (e) => {
		console.log('Received: ' + e.text);
		wsprompt();
	});


	await rl.question("Enter URL (Default: ws://localhost:4444)): ", async (reply) => {
		let url
		if(reply == "") {
			url = "ws://localhost:4444"
		} else {
			url = reply
		}

		console.log('Connecting to ' + url);

		await wsclient.connectTo(url).catch(e => {
			console.log(e)
			process.exit(2);
		})
		console.log('Connected.\r\nEnter data to send. Send \'quit\' to quit.');
		wsprompt();
	})	
}

function prompt(promptName, label, punctuation, defaultVal)
{
  lastPrompt = promptName;
  lastDefault = defaultVal;
  process.stdout.write(`${label} [${defaultVal}] ${punctuation} `);
}
