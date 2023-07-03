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
	if(argv.length !== 3) {
		console.log("Usage: node httpurl.js url");
		console.log("  url the url to fetch");
		console.log("Example: node httpurl.js https://www.google.com");
		process.exit();
	}

	const url = argv[2];

	const http = new ipworksssl.http();


	http.on("Transfer", function(e){
		console.log(e.text.toString());	
	})
	.on("SSLServerAuthentication", function(e) {
		e.accept = true;
	});

	http.setFollowRedirects(1);

	await http.get(url).catch((err) => {
		console.log(err.message);
	});
	process.exit();
}


function prompt(promptName, label, punctuation, defaultVal)
{
  lastPrompt = promptName;
  lastDefault = defaultVal;
  process.stdout.write(`${label} [${defaultVal}] ${punctuation} `);
}
