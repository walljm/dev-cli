using System;

namespace ITPIE.CLI.Models
{
    public class DeviceSearchResult : SearchResult
    {
        [ColumnDisplay(DisplayIndex = 2)]
        public string Vendor { get; set; }

        [ColumnDisplay(DisplayIndex = 3)]
        public string Model { get; set; }

        [ColumnDisplay(DisplayIndex = 4, Name = "OS")]
        public string OsName { get; set; }

        [ColumnDisplay(DisplayIndex = 5, Name = "OS Version")]
        public string OsVersion { get; set; }

        [ColumnDisplay(DisplayIndex = 6)]
        public string ManufacturerType { get; set; }

        [ColumnDisplay(Display = false)]
        public long SysUptimeSeconds { get; set; }

        [ColumnDisplay(DisplayIndex = 7, Name = "System Uptime")]
        public string SystemUptime
        {
            get
            {
                return new TimeSpan(this.SysUptimeSeconds * 10000000).ToString();
            }
        }

        [ColumnDisplay(DisplayIndex = 8)]
        public DateTimeOffset FirstSeen { get; set; }

        [ColumnDisplay(DisplayIndex = 9)]
        public DateTimeOffset LastSeen { get; set; }
    }
}
