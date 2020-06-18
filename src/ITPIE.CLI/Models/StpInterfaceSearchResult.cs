using System;

namespace ITPIE.CLI.Models
{
    public class StpInterfaceSearchResult : SearchResult
    {
        public string ManufacturerType { get; set; }

        public string OsName { get; set; }

        public string BridgeAddress { get; set; }

        public long BridgePriority { get; set; }

        public long InstanceId { get; set; }

        public string Protocol { get; set; }

        public bool IsRoot { get; set; }

        public string InterfaceName { get; set; }

        public string Role { get; set; }

        public string Status { get; set; }

        public string OperPortType { get; set; }

        public string DesignatedBridge { get; set; }

        public long DesignatedBridgePathCost { get; set; }

        public long DesignatedBridgePortNumber { get; set; }

        public long DesignatedBridgePriority { get; set; }

        public bool IsDesignatedRemote { get; set; }

        public EnumerableIntRange VlanRange { get; set; }

        public DateTimeOffset FirstSeen { get; set; }

        public DateTimeOffset LastSeen { get; set; }
    }
}
