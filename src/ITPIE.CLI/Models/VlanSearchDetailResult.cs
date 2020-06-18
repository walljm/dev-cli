using System;

namespace ITPIE.CLI.Models
{
    public class VlanSearchDetailResult : VlanSearchResult
    {
        public string VlanAttribute { get; set; }

        public string VlanFilterAction { get; set; }

        public string VlanFilterDirection { get; set; }

        public EnumerableIntRange VlanFilter { get; set; }

        public DateTimeOffset FirstSeen { get; set; }

        public DateTimeOffset LastSeen { get; set; }
    }
}
