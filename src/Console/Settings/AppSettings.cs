using CommandLine;

namespace CLI.Settings
{
    public class AppSettings
    {
        public ProtectedSettings Protected { get; set; }
        public PublicSettings Public { get; set; }

        public bool IsProtectedValid()
        {
            return this.Protected != null &&
                   this.Protected.ItpiePass != null &&
                   this.Protected.ItpieUser != null;
        }

        public bool IsPublicValid()
        {
            return this.Public != null &&
                   this.Public.ItpieServerUrl != null;
        }

        public const string InvalidProtectedMessage = "A username and password must be populated.  Please run the application with the -s|--store option to securely store the credentials.";
        public const string InvalidPublicMessage = "An ITPIE Url must be provided either by using the -i|--itpie option or from a previously stored value in the public.settings file.";

        /// <summary>
        /// Do the work to parse command line arguments, and store and/or retrieve the Settings from protected and public storage.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Result that indicated whether to exit or continue.</returns>
        public static SetupResult HandleSettings(string[] args)
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

            if (commandLineSettings.SetItpieCredentials)
            {
                storage.GetUsernameAndPassword(settings, false);
            }

            // store settings if that was requested
            if (commandLineSettings.StoreSettings)
            {
                storage.StoreSettings(settings);
                return SetupResult.Fail();
            }

            // everything is done, return the settings as they are and continue to the integration.
            return SetupResult.Success(settings, storage);
        }
    }

    public class SetupResult
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
