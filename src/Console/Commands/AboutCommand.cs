﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using CLI.Models;

namespace CLI.Commands
{
    public class AboutCommand : CommandBase, ICommand
    {
        public override string Name { get { return "about"; } }
        public override string[] Aliases { get { return new[] { "version", "system" }; } }

        public AboutCommand(ContextStack stack)
        {
            this.stack = stack;
        }

        public Task<bool> Run(string cmd)
        {
            Console.WriteLine();
            Console.WriteLine($"  System Information: {getVersion()}");
            Console.WriteLine("  -----------------------------------------------------------------------------");
            this.stack.AppSettings.Public.PrintSettings();
            Console.WriteLine();
            return Task.FromResult(true);
        }

        public Help[] GetHelp()
        {
            return new Help[]{
                new Help
                {
                    Command = this,
                    Description = new List<string>
                    {
                        "Show the current CLI version and other system info."
                    }
                }
            };
        }

        private static string getVersion()
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{v.Major}.{v.Minor}.{v.Revision}";
        }
    }
}
