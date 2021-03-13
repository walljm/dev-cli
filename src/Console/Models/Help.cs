using System.Collections.Generic;
using CLI.Commands;

namespace CLI.Models
{
    public class Help
    {
        public CommandBase Command { get; set; }
        public List<string> Description { get; set; }
    }
}
