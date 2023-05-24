/*
 * IPWorks SSL 2022 .NET Edition - Demo Application
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 */

using System.Collections.Generic;
﻿using System;
using System.Threading.Tasks;
using nsoftware.async.IPWorksSSL;

class soapDemo
{
  private static Soap soap = new nsoftware.async.IPWorksSSL.Soap();

  static async Task Main(string[] args)
  {
    // Process user commands.
    Console.WriteLine("This sample console application converts temperature units using SOAP calls.\nType \"?\" or \"help\" for a list of commands.");
    Console.Write("soap> ");
    string command;
    string[] arguments;

    while (true)
    {
      command = Console.ReadLine();
      arguments = command.Split();

      if (arguments[0] == "?" || arguments[0] == "help")
      {
        Console.WriteLine("Commands:");
        Console.WriteLine(" ?                       display the list of valid commands");
        Console.WriteLine(" help                    display the list of valid commands");
        Console.WriteLine(" f2c <temperature>       convert Fahrenheit value, <temperature>, to Celsius");
        Console.WriteLine(" c2f <temperature>       convert Celsius value, <temperature>, to Fahrenheit");
        Console.WriteLine(" quit                    exit the application");
      }
      else if (arguments[0] == "f2c")
      {
        try
        {
          Console.Write("Converting... ");
          await soap.Reset();
          await soap.Config("MethodNamespacePrefix=");
          soap.URL = "https://www.w3schools.com/xml/tempconvert.asmx";
          soap.MethodURI = "https://www.w3schools.com/xml/";
          soap.Method = "FahrenheitToCelsius";
          soap.ActionURI = soap.MethodURI + soap.Method;
          await soap.AddParam("Fahrenheit", (arguments.Length > 1 ? arguments[1] : "0"));
          await soap.SendRequest();
          soap.XPath = "/Envelope/Body/FahrenheitToCelsiusResponse/FahrenheitToCelsiusResult";
          Console.WriteLine(arguments[1] + "F is " + soap.XText + "C");
        }
        catch (Exception ex)
        {
          Console.WriteLine("Could not convert: " + ex.Message);
        }
      }
      else if (arguments[0] == "c2f")
      {
        try
        {
          Console.Write("Converting... ");
          await soap.Reset();
          await soap.Config("MethodNamespacePrefix=");
          soap.URL = "https://www.w3schools.com/xml/tempconvert.asmx";
          soap.MethodURI = "https://www.w3schools.com/xml/";
          soap.Method = "CelsiusToFahrenheit";
          soap.ActionURI = soap.MethodURI + soap.Method;
          await soap.AddParam("Celsius", (arguments.Length > 1 ? arguments[1] : "0"));
          await soap.SendRequest();
          soap.XPath = "/Envelope/Body/CelsiusToFahrenheitResponse/CelsiusToFahrenheitResult";
          Console.WriteLine(arguments[1] + "C is " + soap.XText + "F");
        }
        catch (Exception ex)
        {
          Console.WriteLine("Could not convert: " + ex.Message);
        }
      }
      else if (arguments[0] == "")
      {
        // Do nothing.
      }
      else if (arguments[0] == "quit" || arguments[0] == "exit")
      {
        break;
      }
      else
      {
        Console.WriteLine("Invalid command.");
      } // End of command checking.

      Console.Write("soap> ");
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