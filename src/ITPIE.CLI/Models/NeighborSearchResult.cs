using System;

namespace ITPIE.CLI.Models
{
    public class NeighborSearchResult : SearchResult
    {
        public string InterfaceName { get; set; }

        public string DiscoveryType { get; set; }

        public int RemoteDeviceId { get; set; }

        public string RemoteDeviceIp { get; set; }

        public string RemoteHostname { get; set; }

        public string RemoteInterfaceName { get; set; }

        public string RemoteInterfaceDesc { get; set; }

        public string RemoteManufacturerType { get; set; }

        public string RemoteVendor { get; set; }

        public string RemoteModel { get; set; }

        public string RemoteOsName { get; set; }

        public string RemoteOsVersion { get; set; }

        public bool RemoteIsUnmanaged { get; set; }

        public DateTimeOffset RemoteFirstSeen { get; set; }

        public DateTimeOffset RemoteLastSeen { get; set; }
    }
}