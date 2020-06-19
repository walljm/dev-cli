using System;

namespace ITPIE.CLI.Models
{
    public class InterfaceSearchResult : SearchResult
    {
        [ColumnDisplay(Name = "VRF Name")]
        public string VrfName { get; set; }

        public string InterfaceName { get; set; }
        public string InterfaceDescription { get; set; }
        public string AdminStatus { get; set; }
        public string OperStatus { get; set; }
        public string AdminSpeed { get; set; }
        public string OperSpeed { get; set; }
        public string AdminDuplex { get; set; }
        public string OperDuplex { get; set; }

        [ColumnDisplay(Display = false)]
        public long LastChangeSeconds { get; set; }

        public string LastChange
        {
            get
            {
                return new TimeSpan(this.LastChangeSeconds * 10000000).ToString();
            }
        }

        [ColumnDisplay(Name = "MAC")]
        public string Mac { get; set; }

        public string AdminVlanPortMode { get; set; }
        public string InterfaceType { get; set; }
        public string FirstSeen { get; set; }
        public string LastSeen { get; set; }

        public override string ToString() => this.Hostname + ":" + this.InterfaceName;
    }
}
