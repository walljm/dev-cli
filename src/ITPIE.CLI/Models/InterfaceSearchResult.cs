namespace ITPIE.CLI.Models
{
    public class InterfaceSearchResult : SearchResult
    {
        public string VrfName { get; set; }
        public string InterfaceName { get; set; }
        public string InterfaceDescription { get; set; }
        public string AdminStatus { get; set; }
        public string OperStatus { get; set; }
        public string AdminSpeed { get; set; }
        public string OperSpeed { get; set; }
        public string AdminDuplex { get; set; }
        public string OperDuplex { get; set; }
        public long LastChangeSeconds { get; set; }
        public string Mac { get; set; }
        public string AdminVlanPortMode { get; set; }
        public string InterfaceType { get; set; }
        public string FirstSeen { get; set; }
        public string LastSeen { get; set; }

        public override string ToString() => this.Hostname + ":" + this.InterfaceName;
    }
}