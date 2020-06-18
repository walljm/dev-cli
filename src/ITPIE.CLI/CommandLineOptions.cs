using CommandLine;

namespace ITPIE.CLI
{
    public class CommandLineOptions
    {
        [Option('c', "command", Required = false, HelpText = "If used will run the command and output the result without entering cli mode.")]
        public string Command { get; set; }

        [Option('u', "user", Required = false, HelpText = "The user to login with.")]
        public string User { get; set; }

        [Option('i', "itpieUrl", Required = false, HelpText = "The url of the itpie server to use for your queries.")]
        public string ItpieUrl { get; set; }
    }
}
