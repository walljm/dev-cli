namespace ITPIE.CLI.Models
{
    public class VlanSearchResult : SearchResult
    {
        public string InterfaceName { get; set; }

        public short VlanId { get; set; }

        public string VlanName { get; set; }

        public string Encapsulation { get; set; }

        public string AdminPortMode { get; set; }

        public string IsBridgingIsIpForwarding { get; set; }

        public string OperStatus { get; set; }

        public long StatusIntervalSeconds { get; set; }
    }
}