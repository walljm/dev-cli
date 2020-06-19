using System;

namespace ITPIE.CLI.Models
{
    public class NeighborSearchResult : SearchResult
    {
        public string InterfaceName { get; set; }

        public string DiscoveryType { get; set; }

        [ColumnDisplay(Display = false)]
        public int RemoteDeviceId { get; set; }

        public string RemoteDeviceIp { get; set; }

        public string RemoteHostname { get; set; }

        public string RemoteInterfaceName { get; set; }

        public string RemoteInterfaceDesc { get; set; }

        public string RemoteManufacturerType { get; set; }

        public string RemoteVendor { get; set; }

        public string RemoteModel { get; set; }
        
        [ColumnDisplay(Name = "Remote OS")]
        public string RemoteOsName { get; set; }

        private string _RemoteOsVersion { get; set; }
        
        [ColumnDisplay(Name = "Remote OS Version")]
        public string RemoteOsVersion
        {
            get
            {
                if (this._RemoteOsVersion.Length > 50)
                {
                    return this._RemoteOsVersion.Substring(0, 50);
                }
                return this._RemoteOsVersion;
            }
            set { this._RemoteOsVersion = value; }
        }

        public bool RemoteIsUnmanaged { get; set; }

        public DateTimeOffset RemoteFirstSeen { get; set; }

        public DateTimeOffset RemoteLastSeen { get; set; }
    }
}
