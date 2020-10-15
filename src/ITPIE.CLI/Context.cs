using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLI.Commands;

namespace CLI
{
    public class Context
    {
        private readonly Dictionary<string, string> environment = new Dictionary<string, string>();

        public string Prompt { get; set; }
        public ICommand DefaultCommand { get; set; }

        public List<ICommand> Commands { get; set; }

        public Context()
        {
        }

        public Context(Dictionary<string, string> environment)
        {
            this.environment = environment;
        }

        #region Commands Handling

        public async Task HandleCommand(string str)
        {
            var cmd = str.TrimStart();

            // change dir is a special case.  you want to always handle it first.
            //var cd = this.GetCommand<ChangeContextCommand>();
            //if (cd.Match(cmd))
            //{
            //    await cd.Run(cmd);
            //    return;
            //}

            if (this.DefaultCommand != null)
            {
                await this.DefaultCommand.Run(str);
                return;
            }

            if (cmd == string.Empty)
            {
                return;  // handle empty commands.
            }

            var torun = this.GetCommand(cmd);
            if (torun != null)
            {
                await torun.Run(cmd);
                return;
            }

            // if you got this far, then the command isn't supported
            Console.WriteLine($"The command \"{cmd}\" isn't a known command.  Please use one of the provided commands below.");
            HelpCommand.WriteHelp(this.Commands);
        }

        public async Task HandlePipeCommand(string str, StringBuilder stdId)
        {
            var cmd = $"{DefaultCommand} {str}".TrimStart();
            foreach (var command in this.Commands)
            {
                if (command.Match(cmd))
                {
                    if (command is IPipableCommand pipe)
                    {
                        await pipe.RunWithPipe(cmd, stdId);
                        return;
                    }

                    await command.Run(cmd);
                    return;
                }
            }

            // if you got this far, then the command isn't supported
            Console.WriteLine($"The piped command \"{cmd}\" isn't a known command. Please use one of the provided commands below.");
            HelpCommand.WriteHelp(this.Commands);
        }

        public T GetCommand<T>()
        {
            return (T)this.Commands.FirstOrDefault(c => c.GetType() == typeof(T));
        }

        public ICommand GetCommand(string cmd)
        {
            foreach (var command in this.Commands.Where(c => c is PipeCommand)) // handle the pipe command first, always.
            {
                if (command.Match(cmd))
                {
                    return command;
                }
            }

            foreach (var command in this.Commands)
            {
                if (command.Match(cmd))
                {
                    return command;
                }
            }

            return null;
        }

        public static List<ICommand> GetDefaultCommands(ContextStack stack)
        {
            return new List<ICommand>
            {
                new HelpCommand(stack),
                new PipeCommand(stack),
                new GrepCommand(),
                new SetCommand(stack),
                new AboutCommand(stack),
                new AllCommand(stack),
                new GitCommand(stack),
            };
        }

        #endregion Commands Handling

        #region Environment

        public string GetEnvVariable(string name)
        {
            var n = $"{Constants.EnvironmentPrefix}_{name}";
            var v = System.Environment.GetEnvironmentVariable(n);
            if (v != null)
            {
                return v;
            }

            if (this.environment.ContainsKey(name))
            {
                return this.environment[name];
            }

            throw new ArgumentException("That variable doesn't exist. :( ");
        }

        public void SetEnvVariable(string name, string val)
        {
            this.environment[name] = val;
        }

        public void PrintEnvironment()
        {
            var kWidth = this.environment.Count > 0 ? this.environment.Max(kvp => kvp.Key.Length) : 0;
            foreach (var kvp in this.environment)
            {
                Console.WriteLine($"  {kvp.Key.PadLeft(kWidth)}: {kvp.Value}");
            }
        }

        #endregion Environment
    }
}
