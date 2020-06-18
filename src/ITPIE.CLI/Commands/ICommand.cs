using System.Text;
using System.Threading.Tasks;

namespace ITPIE.CLI.Commands
{
    public interface ICommand
    {
        string Name { get; }

        Task<bool> Run(string cmd);

        bool Match(string cmd);

        string[] GetHelp();
    }

    public interface IPipableCommand : ICommand
    {
        Task<bool> RunWithPipe(string cmd, StringBuilder stdIn);
    }
}
