using System;
using System.Collections.Generic;

namespace OSLC4Net.Client.Samples
{
    public class CommandLineHelper : Dictionary<string, string>
    {
        public static CommandLineHelper FromArguments(string[] args)
        {
            var dict = new CommandLineHelper();
            foreach (var arg in args)
            {
                // Expected format: /key=value or /key="value"
                if (arg.StartsWith("/"))
                {
                    int eqIndex = arg.IndexOf('=');
                    if (eqIndex > 0)
                    {
                        string key = arg.Substring(1, eqIndex - 1);
                        string value = arg.Substring(eqIndex + 1);
                        // Strip quotes if present
                        if (value.StartsWith("\"") && value.EndsWith("\""))
                        {
                            value = value.Substring(1, value.Length - 2);
                        }
                        dict[key] = value;
                    }
                }
            }
            return dict;
        }
    }
}
