using CommandLine;

namespace CLI
{
    public class CommandLineOptions
    {
        [Option('c', "command", HelpText = "If used will run the command and output the result without entering CLI mode.")]
        public string Command { get; set; }
    }
}
