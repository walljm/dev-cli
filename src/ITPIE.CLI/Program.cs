using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CLI.Commands;
using CommandLine;

namespace CLI
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // parse the command line arguments

            CommandLineOptions opts = null;
            bool stop = false;
            Parser.Default
                .ParseArguments<CommandLineOptions>(args)
                .WithParsed(options =>
                {
                    opts = options;
                })
                .WithNotParsed(errors =>
                {
                    stop = errors.Any(e => e.StopsProcessing);
                });

            // if no arguments then exit.
            if (stop || opts == null)
            {
                return;
            }

            // start up an initial stack of contexts
            var stack = initContextStack();

            // say hello to the user.
            var welcome = new List<string>
            {
                "Welcome to the Walljm Development CLI!",
            };

            foreach (var str in welcome)
            {
                Console.WriteLine(str);
            }

            var about = stack.GetCommand<AboutCommand>();
            await about.Run(about.Name);
            Console.WriteLine();

            // enter the interactive loop.
            while (true)
            {
                var context = stack.Current;
                WritePrompt(stack);

                var cmd = Console.ReadLine();
                if (cmd == "exit") // handle a universal exit command.
                {
                    return;
                }

                await context.HandleCommand(cmd);
            }
        }

        private static void WritePrompt(ContextStack stack)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            // Console.Write($"{stack.Environment[Constants.EnvironmentPath]}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($" {string.Join(Path.DirectorySeparatorChar, stack.Reverse().Select(c => c.Prompt))}> ");
            Console.ResetColor();
        }

        private static ContextStack initContextStack()
        {
            var stack = new ContextStack();
            var mainContext = new Context
            {
                Prompt = $"{Constants.DefaultPrompt}",
                Commands = Context.GetDefaultCommands(stack)
            };
            mainContext.SetEnvVariable(Constants.EnvironmentProjectPath, @"C:\Projects\vae");
            stack.Push(mainContext);
            return stack;
        }
    }
}
