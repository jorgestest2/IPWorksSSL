/*
 * IPWorks SSL 2022 Java Edition- Demo Application
 *
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 *
 */

import java.io.*;

import ipworksssl.*;

public class wsserver extends ConsoleDemo {
  private static Wsserver websocketserver;

  public static void main(String[] args) {
    try {
      websocketserver = new Wsserver();
      System.out.println("************************************************************************");
      System.out.println("* This demo shows how to set up a WebSocket echo server on your system.*");
      System.out.println("************************************************************************\n");

      websocketserver.addWsserverEventListener(new DefaultWsserverEventListener() {
        public void connected(WsserverConnectedEvent e) {
          System.out.println(websocketserver.getConnections().item(e.connectionId).getRemoteHost() + " connected.");
        }

        public void disconnected(WsserverDisconnectedEvent e) {
          System.out.println("Remote host disconnected: " + e.description);
        }

        public void dataIn(WsserverDataInEvent e) {
          System.out.println("Echoing '" + new String(e.text) + "' to " + websocketserver.getConnections().item(e.connectionId).getRemoteHost());
          try {
            websocketserver.send(e.connectionId, e.text);
          } catch (IPWorksSSLException ex) {
            System.out.println("Error echoing data [" + ex.getCode() + "]: " + ex.getMessage());
          }
        }
      });

      websocketserver.setLocalPort(Integer.parseInt(prompt("Local Port",":","777")));
      websocketserver.setListening(true);

      System.out.println("\r\nStarted Listening.");
      System.out.println("\r\nPlease input a command: \r\n1) Send Data \r\n2) Exit");
      System.out.print(">");
      while (true) {
        if (System.in.available() > 0) {
          String command = String.valueOf(read());
          if ("1".equals(command)) {
            String text = prompt("Please input sending data");
            WSConnectionMap connections = websocketserver.getConnections();
            Object[] keys = connections.keySet().toArray();
            if (keys.length > 0) {
              for (int i = 0; i < keys.length; i++) {
                WSConnection connection = (WSConnection) connections.get(keys[i]);
                connection.setDataToSend(text);
              }
              System.out.println("Sending success.");
            } else {
              System.out.println("\r\nNo connected client.");
            }
            System.out.println("\r\nPlease input a command: \r\n1) Send Data \r\n2) Exit");
            System.out.print(">");
          } else if ("2".equals(command)) {
            break;
          }
        }
      }

      websocketserver.setListening(false);
      websocketserver.shutdown();
      System.out.println(">Stopped Listening.");

    } catch (IPWorksSSLException ex) {
      System.out.println("\n\nError: " + ex.getMessage() + "\n");
      System.exit(ex.getCode());
    } catch (Exception ex) {
      System.out.println(ex.getMessage());
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




