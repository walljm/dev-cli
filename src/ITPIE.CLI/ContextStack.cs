using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLI.Commands;

namespace CLI
{
    public class ContextStack : Stack<Context>
    {
        public Context Current { get { return this.Peek(); } }
        
        public async Task HandleCommand(string str)
        {
            await this.Peek().HandleCommand(str);
        }

        public async Task HandlePipeCommand(string str, StringBuilder stdId)
        {
            await this.Peek().HandlePipeCommand(str, stdId);
        }

        public T GetCommand<T>()
        {
            return (T)this.Peek().Commands.FirstOrDefault(c => c.GetType() == typeof(T));
        }

        public ICommand GetCommand(string cmd)
        {
            return this.Peek().GetCommand(cmd);
        }

        public void WriteLine(string str = "")
        {
            if (str.Length == 0)
            {
                Console.WriteLine();
                return;
            }

            var lines = str.Split(new string[] {"\r", "\n"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                Console.WriteLine($" {line}");
            }
        }

        public void WriteError(string str)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($" {str}");
            Console.ResetColor();
        }
    }
}
