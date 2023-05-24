/*
 * IPWorks SSL 2022 .NET Edition - Demo Application
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 */

using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using nsoftware.async.IPWorksSSL;

class sslEchoDemo
{
  private static Sslclient ip;
  private static bool dataReceived = false;

  private static void ip_OnConnected(object sender, SslclientConnectedEventArgs e)
  {
    if (e.Description.ToLower().Equals("ok")) Console.WriteLine("Successfully connected.");
    else Console.WriteLine(e.Description);
  }

  private static void ip_OnDataIn(object sender, SslclientDataInEventArgs e)
  {
    dataReceived = true;
    Console.WriteLine("Received '" + e.Text + "' from " + ip.RemoteHost + ".");
  }

  private static void ip_OnDisconnected(object sender, SslclientDisconnectedEventArgs e)
  {
    Console.WriteLine("Disconnected.");
  }

  private static void ip_OnError(object sender, SslclientErrorEventArgs e)
  {
    Console.WriteLine(e.Description);
  }

  private static void ip_OnSSLServerAuthentication(object sender, SslclientSSLServerAuthenticationEventArgs e)
  {
    if (e.Accept) return;
    Console.Write("Server provided the following certificate:\nIssuer: " + e.CertIssuer + "\nSubject: " + e.CertSubject + "\n");
    Console.Write("The following problems have been determined for this certificate: " + e.Status + "\n");
    Console.Write("Would you like to continue anyways? [y/n] ");
    if (Console.Read() == 'y') e.Accept = true;
  }

  static async Task Main(string[] args)
  {
    ip = new Sslclient();

    if (args.Length < 2)
    {
      Console.WriteLine("usage: sslecho [options] host port");
      Console.WriteLine("Options: ");
      Console.WriteLine("  host       the address of of the remote host");
      Console.WriteLine("  port       the TCP port of the remote host");
      Console.WriteLine("\r\nExample: sslecho localhost 4444");
    }
    else
    {
      ip.OnConnected += ip_OnConnected;
      ip.OnDataIn += ip_OnDataIn;
      ip.OnDisconnected += ip_OnDisconnected;
      ip.OnError += ip_OnError;
      ip.OnSSLServerAuthentication += ip_OnSSLServerAuthentication;

      try
      {
        // Parse arguments into component.
        ip.RemoteHost = args[1];
        ip.RemotePort = int.Parse(args[2]);

        // Attempt to connect to the remote server.
        await ip.Connect();

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
            Console.WriteLine("  send <text>                  send data to the remote host");
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
              await ip.SendLine(textToSend);

              while (!dataReceived)
              {
                await ip.DoEvents();
              }

              dataReceived = false;
            }
            else
            {
              Console.WriteLine("Please supply the text that you would like to send.");
            }
          }
          else if (arguments[0].Equals("quit"))
          {
            await ip.Disconnect();
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

          Console.Write("echoclient> ");
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
        if (i + 1 < args.Length & !args[i + 1].StartsWith("/"))
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