using System.Threading.Tasks;
using CLI.Settings;
using CommandLine;

namespace CLI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // handle settings
            var results = handleSettings(args);
            if (!results.ShouldContinue)
            {
                return;
            }

            // execute the cli
            await new CommandLineInterface(results.Settings, results.Storage).Run();
        }

        /// <summary>
        /// Do the work to parse command line arguments, and store and/or retrieve the Settings from protected and public storage.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Result that indicated whether to exit or continue.</returns>
        internal static SetupResult handleSettings(string[] args)
        {
            // handle the command line settings.
            CommandLineSettings commandLineSettings = null;
            var t = Parser.Default.ParseArguments<CommandLineSettings>(args)
                .WithParsed(o =>
                {
                    commandLineSettings = o;
                })
                .WithNotParsed(o =>
                {
                    if (o.IsHelp())
                    {
                        commandLineSettings = null;
                    }
                });

            if (commandLineSettings == null)
            {
                return SetupResult.Fail();
            }

            // get your storage provider
            var storage = new Storage();
            var settings = storage.RetrieveSettings();

            // if settings were passed in from the cli, then pass them along
            // this should happen before the store settings block
            settings.Public.UpdateWithCommandLineSettings(commandLineSettings);

            // store settings if that was requested
            if (commandLineSettings.StoreSettings)
            {
                storage.GetUsernameAndPassword(settings, true);
                return SetupResult.Fail();
            }

            // everything is done, return the settings as they are and continue to the integration.
            return SetupResult.Success(settings, storage);
        }

        internal class SetupResult
        {
            public bool ShouldContinue { get; set; } = true;
            public AppSettings Settings { get; set; }
            public Storage Storage { get; set; }

            public static SetupResult Fail()
            {
                return new SetupResult { ShouldContinue = false };
            }

            public static SetupResult Success(AppSettings appSettings, Storage storage)
            {
                return new SetupResult { ShouldContinue = true, Settings = appSettings, Storage = storage };
            }
        }
    }
}
