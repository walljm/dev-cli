using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CLI.Commands;
using CLI.Settings;

namespace CLI
{
    public class CommandLineInterface
    {
        private readonly AppSettings appSettings;
        private readonly Storage storage;

        public CommandLineInterface(AppSettings settings, Storage storage)
        {
            this.appSettings = settings;
            this.storage = storage;
        }

        public async Task Run()
        {
            // start up an initial stack of contexts
            var stack = this.initContextStack();

            // say hello to the user.
            var welcome = new List<string>
            {
                "Welcome to the Walljm Development CLI!",
            };

            foreach (var str in welcome)
            {
                ContextStack.WriteLine(str);
            }

            var about = stack.GetCommand<AboutCommand>();
            await about.Run(about.Name);
            ContextStack.WriteLine();

            // enter the interactive loop.
            while (true)
            {
                var context = stack.Current;
                WritePrompt(stack);

                var cmd = Console.ReadLine();
                if (cmd == "exit") // handle a universal exit command.
                {
                    stack.Pop();
                    if (stack.Count == 0)
                    {
                        return;
                    }
                    else
                    {
                        continue;
                    }
                }

                await context.HandleCommand(cmd);
            }
        }

        private static void WritePrompt(ContextStack stack)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($" {string.Join(Path.DirectorySeparatorChar, stack.Reverse().Select(c => c.Prompt))}> ");
            Console.ResetColor();
        }

        private ContextStack initContextStack()
        {
            var stack = new ContextStack(this.appSettings, this.storage);
            stack.Push(new Context()
            {
                Prompt = $"{Constants.DefaultPrompt}",
                Commands = stack.CreateDefaultCommands()
            });
            return stack;
        }
    }
}
