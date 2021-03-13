using System.Threading.Tasks;
using CLI.Settings;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;

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

            // setup your app services
            var appServices = new ServiceCollection();
            appServices.AddSingleton(results.Settings);
            appServices.AddSingleton(results.Storage);
            var appServiceProvider = appServices.BuildServiceProvider();

            // execute the cli
            var integration = ActivatorUtilities.CreateInstance<CommandLineInterface>(appServiceProvider);
            await integration.Run();
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
            // setup data protection so you can store settings safely.
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDataProtection();
            var services = serviceCollection.BuildServiceProvider();

            // get your storage provider
            var storage = ActivatorUtilities.CreateInstance<Storage>(services);
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
