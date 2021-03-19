using System.Threading.Tasks;
using CLI.Settings;

namespace CLI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // handle settings
            var results = AppSettings.HandleSettings(args);
            if (!results.ShouldContinue)
            {
                return;
            }

            // execute the cli
            await new CommandLineInterface(results.Settings, results.Storage).Run();
        }
    }
}
