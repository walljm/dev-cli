using System.Threading.Tasks;

namespace ITPIE.CLI.Commands
{
    public interface ICommand
    {
        string Name {get;}

        Task<bool> Run(string cmd);

        bool Match(string cmd);

        string[] GetHelp();
    }
}