using System;

namespace ITPIE.CLI.Models
{
    public class ArpSearchResult : SearchResult
    {
        public string InterfaceName { get; set; }

        public string Vrf { get; set; }

        public string ArpIp { get; set; }

        public string ArpMac { get; set; }

        public string Registration { get; set; }

        public string MacType { get; set; }

        public bool IsProxyArp { get; set; }

        public DateTimeOffset SeenTo { get; set; }

        public DateTimeOffset SeenFrom { get; set; }
    }
}