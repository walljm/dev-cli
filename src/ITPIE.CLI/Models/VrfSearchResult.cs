﻿namespace ITPIE.CLI.Models
{
    public class VrfSearchResult : SearchResult
    {
        [ColumnDisplay(Name = "VRF")]
        public string VrfName { get; set; }

        [ColumnDisplay(Name = "RD")]
        public string Rd { get; set; }

        public string InterfaceName { get; set; }
        public string InterfaceDescription { get; set; }

        public string InterfaceIp { get; set; }
        public string Mask { get; set; }
        public string NetworkAddress { get; set; }

        public string FHRPStatus { get; set; }
        public string Target { get; set; }

        public override string ToString() => this.VrfName;
    }
}
