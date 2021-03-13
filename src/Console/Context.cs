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
        public string Prompt { get; set; }
        public ICommand DefaultCommand { get; set; }

        public List<ICommand> Commands { get; set; }

        #region Commands Handling

        public async Task HandleCommand(string str)
        {
            var cmd = str.TrimStart();

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
            var cmd = $"{this.DefaultCommand} {str}".TrimStart();
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
            var found_pipe_commands = this.Commands.Where(c => c is PipeCommand && c.Match(cmd)).ToList();
            if (found_pipe_commands.Count == 1)
            {
                return found_pipe_commands.First();
            }
            else if (found_pipe_commands.Count > 1)
            {
                var r = found_pipe_commands.FirstOrDefault(c => cmd.StartsWith($"{c.Name} ") || c.Name == cmd);
                if (r != null)
                    return r;
            }

            var found_commands = this.Commands.Where(c => c.Match(cmd)).ToList();
            if (found_commands.Count == 1)
            {
                return found_commands.First();
            }
            else if (found_commands.Count > 1)
            {
                var r = found_commands.FirstOrDefault(c => cmd.StartsWith($"{c.Name} ") || c.Name == cmd);
                if (r != null)
                    return r;
            }

            return null;
        }

        #endregion Commands Handling
    }
}
