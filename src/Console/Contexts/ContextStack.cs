using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CLI.Commands;
using CLI.Settings;

namespace CLI
{
    public class ContextStack
    {
        private readonly Stack<Context> stack = new();

        public int Count { get { return this.stack.Count; } }
        public Context Current { get { return this.stack.Peek(); } }
        public AppSettings AppSettings { get; private set; }
        public Storage Storage { get; private set; }

        public ContextStack(AppSettings appSettings, Storage storage)
        {
            this.AppSettings = appSettings;
            this.Storage = storage;
        }

        public async Task HandleCommand(string str)
        {
            await this.Current.HandleCommand(str);
        }

        public async Task HandlePipeCommand(string str, StringBuilder stdId)
        {
            await this.Current.HandlePipeCommand(str, stdId);
        }

        public T GetCommand<T>()
        {
            return (T)this.Current.Commands.FirstOrDefault(c => c.GetType() == typeof(T));
        }

        public ICommand GetCommand(string cmd)
        {
            return this.Current.GetCommand(cmd);
        }

        public IEnumerable<Context> Reverse()
        {
            return this.stack.Reverse();
        }

        public void AddContext(Context context)
        {
            this.stack.Push(context);
            Console.Title = this.getTitle();
        }

        public void RemoveContext()
        {
            this.stack.Pop();
            Console.Title = this.getTitle();
        }

        private string getTitle()
        {
            return $"{Assembly.GetExecutingAssembly().GetName().Name}: {string.Join('/', this.stack.Reverse().Select(o => o.Prompt))}";
        }

        public List<ICommand> CreateDefaultCommands()
        {
            return new List<ICommand>
            {
                new HelpCommand(this),
                new PipeCommand(this),
                new GrepCommand(),
                new AboutCommand(this),
                new AllCommand(this),
                new GitCommand(this),
                new PingCommand(this),
                new TestCommand(this),
                new ResolveCommand(this),
                new MdnsCommand(this),
                new ItpieCommand(this)
            };
        }

        public static void Write(string str = "")
        {
            Console.Write(str);
        }

        public static void WriteStart(string str = "")
        {
            Console.Write($" {str}");
        }

        public static void WriteLine(string str = "")
        {
            if (str.Length == 0)
            {
                Console.WriteLine();
                return;
            }

            var lines = str.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                Console.WriteLine($" {line}");
            }
        }

        public static void WriteError(string str)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($" {str}");
            Console.ResetColor();
        }
    }
}
