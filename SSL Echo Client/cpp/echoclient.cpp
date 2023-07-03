/*
 * IPWorks SSL 2022 C++ Edition - Sample Project
 *
 * This sample project demonstrates the usage of IPWorks SSL in a 
 * simple, straightforward way. This is not intended to be a complete 
 * application. Error handling and other checks are simplified for clarity.
 *
 * Copyright (c) 2023 /n software inc. www.nsoftware.com
 */

#include <stdio.h>
#include <stdlib.h>
#include <ctype.h>
#include <string.h>
#include "../../include/ipworksssl.h"
#define LINE_LEN 80

bool dataInReceived;

class MySSLClient : public SSLClient
{
public:
	int FireDisconnected(SSLClientDisconnectedEventParams *e)
	{
		printf("Disconnected from %s.\n", this->GetRemoteHost());
		return 0;
	}

	int FireDataIn(SSLClientDataInEventParams *e)
	{
		printf("Received '%s' from '%s'\n", e->Text, GetRemoteHost());
		dataInReceived = true;
		return 0;
	}

	int FireSSLServerAuthentication(SSLClientSSLServerAuthenticationEventParams *e)
	{
		if (!e->Accept)
		{
			printf("Server provided the following certificate: \n");
			printf("Subject: %s \n", e->CertSubject);
			printf("Issuer: %s \n", e->CertIssuer);
			printf("The following problems have been determined for this certificate:  %s \n", e->Status);
			printf("Would you like to continue or cancel the connection?  [y/n] ");

			char response[LINE_LEN];

			fgets(response, LINE_LEN, stdin);
			response[strlen(response) - 1] = '\0';
			if (strcmp(response, "y") == 0)
				e->Accept = true;
		}

		return 0;
	}

};

int main(int argc, char* argv[])
{
	MySSLClient sslclient;

	printf("*****************************************************************\n");
	printf("* This is a demo to show how to connect to a remote echo server *\n");
	printf("* to send data, and receive the echoed response.                *\n");
	printf("*****************************************************************\n");


	if (argc < 3) {

		fprintf(stderr, "usage: echoclient server port\n");
		fprintf(stderr, "\n");
		fprintf(stderr, "  server  the address of the remote host\n");
		fprintf(stderr, "  port    the TCP port in the remote host\n");
		fprintf(stderr, "\nExample: echoclient localhost 777\n");
		printf("Press enter to continue.");
		getchar();

	}
	else{
		sslclient.SetTimeout(10);

		int ret_code = sslclient.ConnectTo(argv[1], atoi(argv[2]));

		if (ret_code)
		{
			printf("Error connecting: %i - %s\n", ret_code, sslclient.GetLastError());
			goto done;
		}

		char command[LINE_LEN];
		while (true)
		{
			dataInReceived = false;
			printf("\nPlease input command: \r\n- 1 Send Data \r\n- 2 Exit \r\n");
			printf(">");

			fgets(command, LINE_LEN, stdin);
			command[strlen(command) - 1] = '\0';

			if (!strcmp(command, "1"))
			{
				char text[LINE_LEN];
				printf("Please enter data to send: ");
				fgets(text, LINE_LEN, stdin);
				text[strlen(text) - 1] = '\0';
				ret_code = sslclient.SetDataToSend(text, strlen(text));
				if (ret_code)
				{
					printf("Sending failed: %i - %s\n", ret_code, sslclient.GetLastError());
				}
				else
				{
					printf("Waiting for response...\n");
					while (!dataInReceived)
						sslclient.DoEvents();
				}
			}
			else if (!strcmp(command, "2"))
			{
				goto done;
			}
			else
			{
				printf("Command not recognized.\n");
			}
		}

	done:
		if (sslclient.GetConnected())
		{
			sslclient.Disconnect();
		}
		printf("Exiting... (press enter)\n");
		getchar();

		return 0;
	}

}


