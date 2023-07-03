/*
 * IPWorks SSL 2022 .NET Edition - Sample Project
 *
 * This sample project demonstrates the usage of IPWorks SSL in a 
 * simple, straightforward way. This is not intended to be a complete 
 * application. Error handling and other checks are simplified for clarity.
 *
 * Copyright (c) 2023 /n software inc. www.nsoftware.com
 */

using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using nsoftware.async.IPWorksSSL;

class sslEchoDemo
{
  private static Sslserver server;

  private static void server_OnConnected(object sender, SslserverConnectedEventArgs e)
  {
    Console.WriteLine(server.Connections[e.ConnectionId].RemoteHost + " has connected - " + e.Description + ".");
    server.Connections[e.ConnectionId].EOL = "\r\n";
  }

  private static async void server_OnDataIn(object sender, SslserverDataInEventArgs e)
  {
    Console.WriteLine("Echoing '" + e.Text + "' back to client " + server.Connections[e.ConnectionId].RemoteHost + ".");
    await server.SendLine(e.ConnectionId, e.Text);
  }

  private static void server_OnDisconnected(object sender, SslserverDisconnectedEventArgs e)
  {
    Console.WriteLine(server.Connections[e.ConnectionId].RemoteHost + " has disconnected - " + e.Description + ".");
  }

  private static void server_OnError(object sender, SslserverErrorEventArgs e)
  {
    Console.WriteLine(e.Description);
  }

  static async Task Main(string[] args)
  {
    server = new Sslserver();

    if (args.Length < 1)
    {
      Console.WriteLine("usage: sslecho port filename password");
      Console.WriteLine("Options: ");
      Console.WriteLine("  port       the TCP port to listen on");
      Console.WriteLine("  filename   the path to the file containing certificates and optional private keys");
      Console.WriteLine("  password the password for the certificate store file. If the provided test file is used (test.pfx), set the password to \"test\"");
      Console.WriteLine("\r\nExample: sslecho 4444 ../../../test.pfx test");
    }
    else
    {
      server.OnConnected += server_OnConnected;
      server.OnDataIn += server_OnDataIn;
      server.OnDisconnected += server_OnDisconnected;
      server.OnError += server_OnError;

      try
      {
        // Parse arguments into component.
        server.LocalPort = int.Parse(args[1]);

        server.SSLCert = new Certificate(CertStoreTypes.cstAuto, args[2], args[3], "*");

        // Start listening for connections.
        await server.StartListening();

        // Process user commands.
        Console.WriteLine("Type \"?\" for a list of commands.");
        string command;
        string[] arguments;

        while (true)
        {
          command = Console.ReadLine();
          arguments = command.Split();

          if (arguments[0].Equals("?"))
          {
            Console.WriteLine("Commands: ");
            Console.WriteLine("  ?                            display the list of valid commands");
            Console.WriteLine("  send <text>                  send data to connected clients");
            Console.WriteLine("  quit                         exit the application");
          }
          else if (arguments[0].Equals("send"))
          {
            if (arguments.Length > 1)
            {
              string textToSend = "";
              for (int i = 1; i < arguments.Length; i++)
              {
                if (i < arguments.Length - 1) textToSend += arguments[i] + " ";
                else textToSend += arguments[i];
              }
              foreach (Connection connection in server.Connections.Values)
              {
                await server.SendLine(connection.ConnectionId, textToSend);
              }
            }
            else
            {
              Console.WriteLine("Please supply the text that you would like to send.");
            }
          }
          else if (arguments[0].Equals("quit"))
          {
            await server.Shutdown();
            break;
          }
          else if (arguments[0].Equals(""))
          {
            // Do nothing.
          }
          else
          {
            Console.WriteLine("Invalid command.");
          }

          Console.Write("echoserver> ");
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
      Console.WriteLine("Press any key to exit...");
      Console.ReadKey();
    }
  }
}



class ConsoleDemo
{
  public static Dictionary<string, string> ParseArgs(string[] args)
  {
    Dictionary<string, string> dict = new Dictionary<string, string>();

    for (int i = 0; i < args.Length; i++)
    {
      // If it starts with a "/" check the next argument.
      // If the next argument does NOT start with a "/" then this is paired, and the next argument is the value.
      // Otherwise, the next argument starts with a "/" and the current argument is a switch.

      // If it doesn't start with a "/" then it's not paired and we assume it's a standalone argument.

      if (args[i].StartsWith("/"))
      {
        // Either a paired argument or a switch.
        if (i + 1 < args.Length && !args[i + 1].StartsWith("/"))
        {
          // Paired argument.
          dict.Add(args[i].TrimStart('/'), args[i + 1]);
          // Skip the value in the next iteration.
          i++;
        }
        else
        {
          // Switch, no value.
          dict.Add(args[i].TrimStart('/'), "");
        }
      }
      else
      {
        // Standalone argument. The argument is the value, use the index as a key.
        dict.Add(i.ToString(), args[i]);
      }
    }
    return dict;
  }
}