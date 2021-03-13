using CommandLine;

namespace CLI.Settings
{
    /// <summary>
    /// This is the class used to store settings that get passed in via command line flags.
    ///
    /// Its important that these all default to null.  for non nullable primative types, use the c# nullable indicator "?" like so "int?".
    /// </summary>
    public class CommandLineSettings
    {
        
        [Option('p', "itpie-project", Required = false, HelpText = "The path to the ITPIE Project (e.g. c:\\projects\\vae\\operations)")]
        public string ItpieProjectPath { get; set; }

        [Option('i', "itpie-url", Required = false, HelpText = "The URL of the ITPIE server (e.g. https://youritpiedomain.com)")]
        public string ItpieApiUrl { get; set; }

        [Option('a', "accept-certs", Required = false, HelpText = "If set, will allow requests to servers that have certificate validation errors.")]
        public bool? AcceptAllCerts { get; set; }

        [Option('s', "store", Required = false, HelpText = "If set, will request a username and password and store them along with the itpie url in encrypted storage for accessing ITPIE.")]
        public bool StoreSettings { get; set; } = false;
        
        [Option('c', "command", HelpText = "If used will run the command and output the result without entering CLI mode.")]
        public string Command { get; set; }
    }
}
