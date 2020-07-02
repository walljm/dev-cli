using CommandLine;

namespace ITPIE.CLI
{
    public class CommandLineOptions
    {
        [Option('c', "command", HelpText = "If used will run the command and output the result without entering CLI mode.")]
        public string Command { get; set; }

        [Option('u', "user", HelpText = "The user to login with.")]
        public string User { get; set; }

        [Option('i', "itpieUrl", HelpText = "The url of the itpie server to use for your queries.")]
        public string ItpieUrl { get; set; }

        [Option('x', "acceptAllCertificates", Default = false, HelpText = "If you're ITPIE server is using a self signed cert, this will allow interaction.")]
        public bool AcceptAllCertificates { get; set; }

        [Option('j', "json", Default = false, HelpText = "Output the command output as JSON.  Only works if you're passing commands in via -c|--command")]
        public bool OutputAsJson { get; set; }
    }
}
