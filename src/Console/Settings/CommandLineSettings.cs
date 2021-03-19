using CommandLine;

namespace CLI.Settings
{
    /// <summary>
    /// This is the class used to store settings that get passed in via command line flags.
    ///
    /// Its important that anything passed to the <see cref="AppSettings"/> be a nullable type,
    ///  and that they be set to null initially.
    ///
    /// For non nullable primative types, use the c# nullable indicator "?" like so "int?".
    /// </summary>
    public class CommandLineSettings
    {
        [Option("itpie-project", HelpText = "The path to the ITPIE Project (e.g. c:\\projects\\vae\\operations)")]
        public string ItpieProjectPath { get; set; }

        [Option("itpie-url", HelpText = "The URL of the ITPIE server (e.g. https://youritpiedomain.com)")]
        public string ItpieApiUrl { get; set; }
        
        [Option("itpie-credentials", HelpText = "If passed, will request a username and password to use when accessing ITPIE.")]
        public bool SetItpieCredentials { get; set; } = false;

        [Option('a', "accept-certs", HelpText = "If set, will allow HTTP requests to servers that have certificate validation errors.")]
        public bool? AcceptAllCerts { get; set; }

        [Option('s', "store", HelpText = "If set, will store any passed in settings for later use.")]
        public bool StoreSettings { get; set; } = false;
        
        [Option("prompt", HelpText = "Will set the prompt used by the cli.")]
        public string DefaultPrompt { get; set; }

        [Option('c', "command", HelpText = "If used will run the command and output the result without entering CLI mode.")]
        public string Command { get; set; }
    }
}
