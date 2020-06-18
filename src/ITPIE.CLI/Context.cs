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
            foreach (var command in this.Commands)
            {
                if (command.Match(cmd))
                {
                    await command.Run(cmd);
                    return;
                }
            }
        }

        public T GetCommand<T>()
        {
            return (T)this.Commands.FirstOrDefault(c => c.GetType() == typeof(T));
        }

        public void WritePrompt()
        {
            Console.Write($"{this.Prompt} ");
        }
    }
}
