using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITPIE.CLI.Commands;

namespace ITPIE.CLI
{
    public class Context
    {
        public Dictionary<string, object> Variables { get; set; }
        public string Prompt { get; set; }
        public List<ICommand> Commands { get; set; }

        public async Task HandleCommand(string cmd)
        {
            if (cmd == "help")
            {
                Console.WriteLine();
                Console.WriteLine($"   Available Commands");
                Console.WriteLine($"   --------------------------------------------------------------------------------");
                foreach (var command in this.Commands)
                {
                    var helps = command.GetHelp();
                    foreach (var help in helps)
                    {
                        Console.WriteLine($"   {help}");
                    }
                }
                Console.WriteLine("   exit | exits the cli application");
                Console.WriteLine();
                return;
            }

            foreach (var command in Commands)
            {
                if (command.Match(cmd))
                {
                    await command.Run(cmd);
                    return;
                }
            }
        }

        public string TabCompletion(string cmd)
        {
            return Commands.Select(c => c.Name).FirstOrDefault(c => c.StartsWith(cmd));
        }

        public T GetCommand<T>()
        {
            return (T)Commands.FirstOrDefault(c => c.GetType() == typeof(T));
        }

        public void WritePrompt()
        {
            Console.Write($"{Prompt} ");
        }
    }
}
