using System;

namespace ITPIE.CLI.Commands.Find
{
    public class Term
    {
        public Func<string, bool> Is { get; set; }
        public string Name { get; set; }
    }
}
