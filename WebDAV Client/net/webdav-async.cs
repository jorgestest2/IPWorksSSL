/*
 * IPWorks SSL 2022 .NET Edition - Demo Application
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 */

using System.Collections.Generic;
﻿using System;
using System.Threading.Tasks;
using nsoftware.async.IPWorksSSL;

class webdavDemo
{
  private static Webdav webdav = new nsoftware.async.IPWorksSSL.Webdav();

  static async Task Main(string[] args)
  {
    if (args.Length < 2)
    {
      Console.WriteLine("usage: webdav /u username /p password");
      Console.WriteLine("  username   the username to login");
      Console.WriteLine("  password   the password to login");
      Console.WriteLine("\r\nExample: webdav /u username /p password");
    }
    else
    {
      webdav.OnConnected += webdav_OnConnected;
      webdav.OnConnectionStatus += webdav_OnConnectionStatus;
      webdav.OnDisconnected += webdav_OnDisconnected;
      webdav.OnSSLServerAuthentication += webdav_OnSSLServerAuthentication;
      webdav.OnTransfer += webdav_OnTransfer;
      webdav.OnDirList += webdav_OnDirList;

      try
      {
        Dictionary<string, string> myArgs = ConsoleDemo.ParseArgs(args);

        // Parse arguments into component.
        webdav.User = myArgs["u"];
        webdav.Password = myArgs["p"];

        // Process user commands.
        Console.WriteLine("Type \"?\" or \"help\" for a list of commands.");
        Console.Write("webdav> ");
        string command;
        string[] arguments;

        while (true)
        {
          command = Console.ReadLine();
          arguments = command.Split();

          if (arguments[0] == "?" || arguments[0] == "help")
          {
            Console.WriteLine("Commands: ");
            Console.WriteLine("  ?                                      display the list of valid commands");
            Console.WriteLine("  help                                   display the list of valid commands");
            Console.WriteLine("  ls <resource uri>                      list the contents of the specified directory");
            Console.WriteLine("  make <resource uri>                    make a new directory at the specified location (ex. make https://localhost:443/directoryName)");
            Console.WriteLine("  move <source uri> <destination uri>    move a specified resource to a new location (ex. move https://localhost:443/oldFolder/file.txt https://localhost:443/newFolder/file.txt)");
            Console.WriteLine("  get <resource uri>                     get a specified resource");
            Console.WriteLine("  delete <resource uri>                  delete a specified resource");
            Console.WriteLine("  put <local file> <resource uri>        send data to the server");
            Console.WriteLine("  quit                                   exit the application");
          }
          else if (arguments[0] == "ls")
          {
            if (arguments.Length > 1) await webdav.ListDirectory(arguments[1]);
          }
          else if (arguments[0] == "make")
          {
            if (arguments.Length > 1) await webdav.MakeDirectory(arguments[1]);
          }
          else if (arguments[0] == "move")
          {
            if (arguments.Length > 2) await webdav.MoveResource(arguments[1], arguments[2]);
          }
          else if (arguments[0] == "get")
          {
            if (arguments.Length > 1) await webdav.GetResource(arguments[1]);
          }
          else if (arguments[0] == "delete")
          {
            if (arguments.Length > 1) await webdav.DeleteResource(arguments[1]);
          }
          else if (arguments[0] == "put")
          {
            if (arguments.Length > 2)
            {
              webdav.LocalFile = arguments[1];
              await webdav.PutResource(arguments[2]);
            }
          }
          else if (arguments[0] == "quit" || arguments[0] == "exit")
          {
            break;
          }
          else if (arguments[0] == "")
          {
            // Do nothing.
          }
          else
          {
            Console.WriteLine("Invalid command.");
          } // End of command checking.

          Console.Write("webdav> ");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error: " + ex.Message);
      }
    }
  }

  #region "Events"

  private static void webdav_OnSSLServerAuthentication(object sender, WebdavSSLServerAuthenticationEventArgs e)
  {
    // This will trust all certificates and is not recommended for production use.
    e.Accept = true;
  }

  private static void webdav_OnConnected(object sender, WebdavConnectedEventArgs e)
  {
    Console.WriteLine("Server connected");
  }

  private static void webdav_OnConnectionStatus(object sender, WebdavConnectionStatusEventArgs e)
  {
    Console.WriteLine("Status code " + e.StatusCode + ": " + e.Description);
  }

  private static void webdav_OnDisconnected(object sender, WebdavDisconnectedEventArgs e)
  {
    Console.WriteLine("Server disconnected");
  }

  private static void webdav_OnTransfer(object sender, WebdavTransferEventArgs e)
  {
    Console.WriteLine("Resource being received from server (in full text): \n" +
                                    "========================================= \n" + e.Text);
  }

  private static void webdav_OnDirList(object sender, WebdavDirListEventArgs e)
  {
    Console.WriteLine(e.DisplayName + ": " + e.ResourceURI);
  }

  #endregion
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