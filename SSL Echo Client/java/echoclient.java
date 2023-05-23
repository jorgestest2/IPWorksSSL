/*
 * IPWorks SSL 2022 Java Edition- Demo Application
 *
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 *
 */

import java.io.*;

import ipworksssl.*;

public class echoclient extends ConsoleDemo {

	private static Sslclient sslclient;

	public static void main(String[] args) {
		if (args.length != 2) {
			System.out.println("usage: echoclient server port");
			System.out.println("");
			System.out.println("  server  the address of the remote host");
			System.out.println("  port    the TCP port in the remote host");
			System.out.println("\r\nExample: echoclient localhost 777");
		} else {
			try {
				sslclient = new Sslclient();
				System.out.println("*************************************************************************************************************");
				System.out.println("* This is a demo to show how to connect to a remote echo server, send data, and receive the echoed response.*");
				System.out.println("*************************************************************************************************************\n");
				sslclient.addSslclientEventListener(new DefaultSslclientEventListener() {
					public void SSLServerAuthentication(SslclientSSLServerAuthenticationEvent e) {
						e.accept = true;
					}

					public void connected(SslclientConnectedEvent e) {
						System.out.println("\r\n" + sslclient.getRemoteHost() + " has connected.");
						System.out.print(">");
					}

					public void dataIn(SslclientDataInEvent e) {
						System.out.println("Received " + new String(e.text) + " from " + sslclient.getRemoteHost());
						System.out.print(">");
					}

					public void disconnected(SslclientDisconnectedEvent e) {
						System.out.println("Disconnected " + e.description + " from " + sslclient.getRemoteHost() + ".");
						System.out.print(">");
					}
				});
				sslclient.setRemoteHost(args[0]);
				sslclient.setRemotePort(Integer.parseInt(args[1]));
				sslclient.connect();
				sslclient.doEvents();
				if (sslclient.isConnected()) {
					System.out.println("\r\nPlease input command: \r\n- 1 Send Data \r\n- 2 Exit");
					System.out.print(">");

					while (true) {
						if (System.in.available() > 0) {
							String command = String.valueOf(read());
							if ("1".equals(command)) {
								sslclient.setDataToSend(prompt("Please input sending data") + "\r\n");
								System.out.println("Sending success.");
								System.out.println("\r\nPlease input command: \r\n- 1 Send Data \r\n- 2 Exit");
								System.out.print(">");
							} else if ("2".equals(command)) {
								break;
							}
						}
					}
					sslclient.disconnect();
				} else {
					System.out.println("\r\nCan't connect to server '" + args[0] + ":" + args[1] + "'");
				}
			} catch (Exception ex) {
				System.out.println(ex.getMessage());
			}
		}
	}
}

class ConsoleDemo {
  private static BufferedReader bf = new BufferedReader(new InputStreamReader(System.in));

  static String input() {
    try {
      return bf.readLine();
    } catch (IOException ioe) {
      return "";
    }
  }
  static char read() {
    return input().charAt(0);
  }

  static String prompt(String label) {
    return prompt(label, ":");
  }
  static String prompt(String label, String punctuation) {
    System.out.print(label + punctuation + " ");
    return input();
  }

  static String prompt(String label, String punctuation, String defaultVal)
  {
	System.out.print(label + " [" + defaultVal + "] " + punctuation + " ");
	String response = input();
	if(response.equals(""))
		return defaultVal;
	else
		return response;
  }

  static char ask(String label) {
    return ask(label, "?");
  }
  static char ask(String label, String punctuation) {
    return ask(label, punctuation, "(y/n)");
  }
  static char ask(String label, String punctuation, String answers) {
    System.out.print(label + punctuation + " " + answers + " ");
    return Character.toLowerCase(read());
  }

  static void displayError(Exception e) {
    System.out.print("Error");
    if (e instanceof IPWorksSSLException) {
      System.out.print(" (" + ((IPWorksSSLException) e).getCode() + ")");
    }
    System.out.println(": " + e.getMessage());
    e.printStackTrace();
  }
}




