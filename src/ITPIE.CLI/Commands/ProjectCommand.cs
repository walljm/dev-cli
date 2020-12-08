using System.IO;
using System.Threading.Tasks;
using CLI.Models;

namespace CLI.Commands
{
    public class ProjectCommand : CommandBase, ICommand
    {
        public override string Name { get { return "project"; } }
        public override string[] Aliases { get { return new string[] { }; } }
        public string Project { get; }

        public ProjectCommand(ContextStack stack, string project)
        {
            this.stack = stack;
            this.Project = project;
        }

        public async Task<bool> Run(string cmd)
        {
            var ctx = this.stack.Current;
            var oldPath = ctx.GetEnvVariable(Constants.EnvironmentProjectPath);

            ctx.SetEnvVariable(Constants.EnvironmentProjectPath, Path.Combine(oldPath, Project));
            var torun = ctx.GetCommand(cmd);
            if (torun != null)
            {
                await torun.Run(cmd);
            }
            this.stack.WriteLine();

            ctx.SetEnvVariable(Constants.EnvironmentProjectPath, oldPath);
            return true;
        }

        public Help[] GetHelp()
        {
            return new Help[]{
            };
        }
    }
}
