using System;

namespace ITPIE.CLI.Models
{
    public class StpSearchResult : SearchResult
    {
        public string Model { get; set; }

        public string Vendor { get; set; }

        public string ManufacturerType { get; set; }

        public string OsName { get; set; }

        public string OsVersion { get; set; }

        public long InstanceId { get; set; }

        public string BridgeAddress { get; set; }

        public long BridgePriority { get; set; }

        public long ForwardDelaySeconds { get; set; }

        public long HelloTimeSeconds { get; set; }

        public long MaxAgeSeconds { get; set; }

        public string MstName { get; set; }

        public long MstRevision { get; set; }

        public string Protocol { get; set; }

        public string ProtocolVersion { get; set; }

        public string AdminStatus { get; set; }

        public bool IsRoot { get; set; }

        public bool IsTcnFlagSet { get; set; }

        public bool IsTcnFlagDetected { get; set; }

        public long LastTopologyChangeSeconds { get; set; }

        public string RegionalRootBridge { get; set; }

        public long RegionalRootPriority { get; set; }

        public string RootBridge { get; set; }

        public string RootHostname { get; set; }

        public string RootIp { get; set; }

        public long RootPriority { get; set; }

        public long RootPathCost { get; set; }

        public string RootPort { get; set; }

        public EnumerableIntRange VlanRange { get; set; }

        public DateTimeOffset FirstSeen { get; set; }

        public DateTimeOffset LastSeen { get; set; }
    }
}
