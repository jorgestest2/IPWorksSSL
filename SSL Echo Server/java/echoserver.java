/*
 * IPWorks SSL 2022 Java Edition- Demo Application
 *
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 *
 */

import java.io.*;

import ipworksssl.*;

public class echoserver extends ConsoleDemo {

	private static Sslserver sslserver;

	public static void main(String[] args) {
		if (args.length != 3) {
			System.out.println("usage: echoserver port filename password");
			System.out.println("");
			System.out.println("  port     the TCP port in the local host where the component listens");
			System.out.println("  filename the path to the file containing certificates and optional private keys");
			System.out.println("  password the password for the certificate store file. If the provided test file is used (test.pfx), set the password to \"test\"");
			System.out.println("\r\nExample: echoserver 777 test.pfx test");
		} else {
			try {
				sslserver = new Sslserver();
				System.out.println("*****************************************************************************************");
				System.out.println("* This demo shows how to set up an echo server on your computer.                        *");
				System.out.println("*****************************************************************************************\n");

				sslserver.addSslserverEventListener(new DefaultSslserverEventListener() {
					public void SSLClientAuthentication(SslserverSSLClientAuthenticationEvent arg0) {
						arg0.accept = true;
					}

					public void connected(SslserverConnectedEvent e) {
						ConnectionMap connections = sslserver.getConnections();
						Connection connection = (Connection) connections.get(e.connectionId);
						System.out.println(connection.getRemoteHost() + " has connected.");
						System.out.print(">");
						try {
							connection.setEOL("\r\n");
						} catch (IPWorksSSLException e1) {
						}
					}

					public void dataIn(SslserverDataInEvent e) {
						try {
							ConnectionMap connections = sslserver.getConnections();
							Connection connection = (Connection) connections.get(e.connectionId);
							connection.setDataToSend(e.text);
							System.out.println("Echoing '" + new String(e.text) + "' to client " + connection.getRemoteHost() + ".");
							System.out.print(">");
						} catch (IPWorksSSLException e1) {
							e1.printStackTrace();
						}
					}

					public void disconnected(SslserverDisconnectedEvent e) {
						System.out.println("Disconnected " + e.description + " from " + e.connectionId + ".");
						System.out.print(">");
					}
				});

				sslserver.setSSLCert(new Certificate(4, args[1], args[2], "*"));
				sslserver.setLocalPort(Integer.parseInt(args[0]));
				sslserver.setListening(true);

				System.out.println("\r\nStarted Listening.");
				System.out.println("\r\nPlease input command: \r\n- 1 Send Data \r\n- 2 Exit");
				System.out.print(">");

				while (true) {
					if (System.in.available() > 0) {
						String command = String.valueOf(read());
						if ("1".equals(command)) {
							String text = prompt("Please input sending data");
							ConnectionMap connections = sslserver.getConnections();
							Object[] keys = connections.keySet().toArray();
							if (keys.length > 0) {
								for (int i = 0; i < keys.length; i++) {
									Connection connection = (Connection) connections.get(keys[i]);
									connection.setDataToSend(text);
								}
								System.out.println("Sending success.");
							} else {
								System.out.println("\r\nNo connected client.");
							}
							System.out.println("\r\nPlease input command: \r\n- 1 Send Data \r\n- 2 Exit");
							System.out.print(">");
						} else if ("2".equals(command)) {
							break;
						}
					}
				}
				sslserver.setListening(false);
				sslserver.shutdown();
				System.out.println(">Stopped Listening.");
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




