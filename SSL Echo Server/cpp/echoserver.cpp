/*
 * IPWorks SSL 2022 C++ Edition - Demo Application
 *
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 *
 */

#include <stdio.h>
#include <stdlib.h>
#include <ctype.h>
#include <string.h>

#include "../../include/ipworksssl.h"
#define LINE_LEN 80

class MySSLServer : public SSLServer
{
	int FireConnected(SSLServerConnectedEventParams *e)
	{
		printf("%s has connected.\n", this->GetRemoteHost(e->ConnectionId));
		return 0;
	}

	int FireDisconnected(SSLServerDisconnectedEventParams *e)
	{
		printf("Disconnected from %s.\n", this->GetRemoteHost(e->ConnectionId));
		return 0;
	}

	int FireDataIn(SSLServerDataInEventParams *e)
	{
		printf("%s - Echoing '%s' back to client.\n", this->GetRemoteHost(e->ConnectionId), e->Text);
		this->SetDataToSend(e->ConnectionId, e->Text, strlen(e->Text));
		return 0;
	}
};

int main(int argc, char* argv[])
{
	MySSLServer sslserver;

	char buffer[LINE_LEN];

	printf("*****************************************************************\n");
	printf("* This demo shows how to set up an echo server using SSLServer. *\n");
	printf("*****************************************************************\n");

	if (argc < 2) {

		fprintf(stderr, "usage: echoserver port filename password\n");
		fprintf(stderr, "\n");
		fprintf(stderr, "  port       the TCP port in the local host where the component listens\n");
		fprintf(stderr, "  filename   the path to the file containing certificates and optional private keys.\n");
		fprintf(stderr, "  password   the password for the certificate store file. If the provided test file is used (test.pfx), set the password to \"test\"\n");
		fprintf(stderr, "\nExample:       echoserver 777 ./test.pfx test\n\n");
		printf("Press enter to continue.");
		getchar();

	}
	else{
		sslserver.SetSSLCertStoreType(CST_AUTO);
		sslserver.SetSSLCertEncoded(argv[2], strlen(argv[2]));
		sslserver.SetSSLCertStorePassword(argv[3]);
		sslserver.SetSSLCertSubject("*");

		sslserver.SetLocalPort(atoi(argv[1]));

		int ret_code = sslserver.StartListening();

		if (ret_code)
		{
			printf("Error: %i - %s\n", ret_code, sslserver.GetLastError());
			goto done;
		}

		printf("Listening...\n");

		while (true)
		{
			sslserver.DoEvents();
		}

	done:
		if (sslserver.GetListening())
		{
			sslserver.StartListening();
		}

		printf("Exiting... (press enter)\n");
		getchar();

		return 0;
	}

}


