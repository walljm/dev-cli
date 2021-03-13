using System.Text;
using System.Threading.Tasks;
using CLI.Models;

namespace CLI.Commands
{
    public interface ICommand
    {
        string Name { get; }

        Task<bool> Run(string cmd);

        bool Match(string cmd);

        Help[] GetHelp();
    }

    public interface IPipableCommand : ICommand
    {
        Task<bool> RunWithPipe(string cmd, StringBuilder stdIn);
    }
}
