/*
 * IPWorks SSL 2022 .NET Edition - Demo Application
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 */

using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using nsoftware.async.IPWorksSSL;

class imapDemo
{
  private static Imap imap1 = new Imap();
  private static int lines = 0;

  private static void imap1_OnSSLServerAuthentication(object sender, ImapSSLServerAuthenticationEventArgs e)
  {
    if (e.Accept) return;
    Console.WriteLine("The server's SSL certificate was not trusted by the system. Server provided the following certificate...");
    Console.WriteLine("     Issuer:  " + e.CertIssuer);
    Console.WriteLine("     Subject: " + e.CertSubject);
    Console.WriteLine("  The following problems have been determined for this certificate: " + e.Status);
    Console.Write("Would you like to continue anyways? [y/n] ");
    if (Console.ReadLine() == "y") e.Accept = true;
  }

  private static void imap1_OnMailboxList(object sender, ImapMailboxListEventArgs e)
  {
    Console.WriteLine(e.Mailbox);
    lines++;
    if (lines == 22)
    {
      Console.Write("Press enter to continue...");
      Console.ReadLine();
      lines = 0;
    }
  }

  private static void imap1_OnMessageInfo(object sender, ImapMessageInfoEventArgs e)
  {
    Console.Write(e.MessageId + "  ");
    Console.Write(e.Subject + "  ");
    Console.Write(e.MessageDate + "  ");
    Console.WriteLine(e.From);
    lines++;
    if (lines == 22)
    {
      Console.Write("Press enter to continue...");
      Console.ReadLine();
      lines = 0;
    }
  }

  private static void imap1_OnTransfer(object sender, ImapTransferEventArgs e)
  {
    Console.Write(e.Text);
    lines++;
    if (lines == 22)
    {
      Console.Write("Press enter to continue...");
      Console.ReadLine();
      lines = 0;
    }
  }

  static async Task Main(string[] args)
  {
    if (args.Length < 3) {

      Console.WriteLine("usage: imap server username password");
      Console.WriteLine("  server    the name or address of the mail server (IMAP server)");
      Console.WriteLine("  username  the user name used to authenticate to the MailServer ");
      Console.WriteLine("  password  the password used to authenticate to the MailServer ");
      Console.WriteLine("\nExample: imap 127.0.0.1 username password");
      Console.WriteLine("Press enter to continue.");
      Console.Read();
    }
    else
    {
      try
      {
        imap1.OnSSLServerAuthentication += imap1_OnSSLServerAuthentication;
        imap1.OnMailboxList += imap1_OnMailboxList;
        imap1.OnMessageInfo += imap1_OnMessageInfo;
        imap1.OnTransfer += imap1_OnTransfer;

        imap1.MailServer = args[args.Length - 3];
        imap1.User = args[args.Length - 2];
        imap1.Password = args[args.Length - 1];
        Console.WriteLine("Connecting.");
        await imap1.Connect();
        DisplayMenu();

        string command;
        string[] argument;
        int msgnum = 1;
        do
        {
          Console.Write("imap> ");
          command = Console.ReadLine();
          argument = command.Split();
          if (argument.Length == 0 || String.IsNullOrEmpty(argument[0]))
            continue;
          switch (argument[0][0])
          {
            case 's':
              try
              {
                if (argument.Length < 2)
                {
                  Console.WriteLine("Must provide a mailbox to select.");
                }
                imap1.Mailbox = argument[1];
                await imap1.SelectMailbox();
              }
              catch(Exception ex)
              {
                Console.WriteLine(ex.Message);
              }
              break;
            case 'h':
              try
              {
                if (imap1.MessageCount > 0)
                {
                  if (imap1.MessageSet == "") imap1.MessageSet = "1:" + imap1.MessageCount;
                  await imap1.FetchMessageInfo();
                }
                else
                {
                  Console.WriteLine("No messages in this mailbox.");
                }
              }
              catch(Exception ex)
              {
                Console.WriteLine(ex.Message);
              }
              break;
            case 'l':
              try
              {
                imap1.Mailbox = argument.Length < 2 ? "*" : imap1.Mailbox = argument[1];
                await imap1.ListMailboxes();
              }
              catch(Exception ex)
              {
                Console.WriteLine(ex.Message);
              }
              break;
            case 'n':
              try
              {
                msgnum++;
                imap1.MessageSet = msgnum.ToString();
                await imap1.FetchMessageText();
              }
              catch(Exception ex)
              {
                Console.WriteLine(ex.Message);
              }
              break;
            case 'q':
              await imap1.Disconnect();
              return;
            case 'v':
              try
              {
                if (argument.Length < 2)
                { 
                  Console.WriteLine("Message number required.");
                  continue;
                }
                msgnum = int.Parse(argument[1]);
                imap1.MessageSet = argument[1];
                await imap1.FetchMessageText();
              }
              catch(Exception ex)
              {
                Console.WriteLine(ex.Message);
              }
              break;
            case '?':
              DisplayMenu();
              break;
            default: // allow user to enter only the number of the message they
                     // want to view
              try
              {
                msgnum = int.Parse(command);
                imap1.MessageSet = command;
                await imap1.FetchMessageText();
              }
              catch (FormatException e)
              {
                Console.WriteLine("Bad command / Not implemented in demo.");
              }
              break;
          }
        } while (true);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
      Console.WriteLine("Press any key to exit...");
      Console.ReadKey();
    }
  }

  private static void DisplayMenu()
  {
    Console.WriteLine("IMAP Commands");
    Console.WriteLine("  l                   list mailboxes");
    Console.WriteLine("  s <mailbox>         select mailbox");
    Console.WriteLine("  v <message number>  view the content of selected message");
    Console.WriteLine("  n                   goto and view next message");
    Console.WriteLine("  h                   print out active message headers");
    Console.WriteLine("  ?                   display options");
    Console.WriteLine("  q                   quit");
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