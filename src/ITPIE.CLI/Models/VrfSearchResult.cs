namespace ITPIE.CLI.Models
{
    public class VrfSearchResult : SearchResult
    {
        public string VrfName { get; set; }
        public string Rd { get; set; }
        public string InterfaceName { get; set; }
        public string InterfaceDescription { get; set; }

        public string InterfaceIp { get; set; }
        public string Mask { get; set; }
        public string NetworkAddress { get; set; }

        public string FHRPStatus { get; set; }
        public string Target { get; set; }

        public override string ToString() => VrfName;
    }
}