using System;

namespace ITPIE.CLI.Models
{
    public class VlanSearchInterfaceResult : SearchResult
    {
        public string InterfaceName { get; set; }

        public EnumerableIntRange VlanIds { get; set; }

        public string VlanName { get; set; }

        public string AdminPortMode { get; set; }

        public EnumerableIntRange VlanFilter { get; set; }

        public string VlanFilterAction { get; set; }

        public string VlanFilterDirection { get; set; }

        public string VlanAttribute { get; set; }

        public string OperStatus { get; set; }

        [ColumnDisplay(Display = false)]
        public long StatusIntervalSeconds { get; set; }

        public string StatusInterval
        {
            get
            {
                return new TimeSpan(this.StatusIntervalSeconds * 10000000).ToString();
            }
        }

        public DateTimeOffset FirstSeen { get; set; }

        public DateTimeOffset LastSeen { get; set; }
    }
}
