using System;

namespace ITPIE.CLI.Models
{
    public class ArpSearchResult : SearchResult
    {
        public string InterfaceName { get; set; }

        [ColumnDisplay(Name = "VRF")]
        public string Vrf { get; set; }

        [ColumnDisplay(Name = "ARP IP")]
        public string ArpIp { get; set; }

        [ColumnDisplay(Name = "ARP MAC")]
        public string ArpMac { get; set; }

        public string Registration { get; set; }

        [ColumnDisplay(Name = "MAC Type")]
        public string MacType { get; set; }

        [ColumnDisplay(Name = "Is Proxy ARP")]
        public bool IsProxyArp { get; set; }

        public DateTimeOffset SeenTo { get; set; }

        public DateTimeOffset SeenFrom { get; set; }
    }
}
