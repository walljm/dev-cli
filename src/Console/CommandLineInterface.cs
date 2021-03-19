using System;
using System.Collections.Generic;
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
            var stack = new ContextStack(this.appSettings, this.storage);
            stack.AddContext(new Context(this.appSettings.Public.DefaultPrompt, stack.CreateDefaultCommands()));

            // say hello to the user.
            var welcome = new List<string>
            {
                "Welcome to the Walljm Development CLI!",
                "Go forth and be more productive -> "
            };

            foreach (var str in welcome)
            {
                ContextStack.WriteLine(str);
            }

            // tell the user whats up.
            var about = stack.GetCommand<AboutCommand>();
            await about.Run(about.Name);
            ContextStack.WriteLine();

            // enter the interactive loop.
            while (true)
            {
                stack.WritePrompt();

                var cmd = Console.ReadLine();
                if (cmd == "exit") // handle a universal exit command.
                {
                    stack.RemoveContext();
                    if (stack.Count == 0)
                    {
                        return;
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (cmd == "quit")
                {
                    return; // hard exit.
                }

                await stack.Current.HandleCommand(cmd);
            }
        }
    }
}
