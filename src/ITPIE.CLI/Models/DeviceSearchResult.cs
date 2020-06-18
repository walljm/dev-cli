using System;

namespace ITPIE.CLI.Models
{
    public class DeviceSearchResult : SearchResult
    {
        [ColumnDisplay(DisplayIndex = 2)]
        public string Vendor { get; set; }

        [ColumnDisplay(DisplayIndex = 3)]
        public string Model { get; set; }

        [ColumnDisplay(DisplayIndex = 4)]
        public string OsName { get; set; }

        [ColumnDisplay(DisplayIndex = 5)]
        public string OsVersion { get; set; }

        [ColumnDisplay(DisplayIndex = 6)]
        public string ManufacturerType { get; set; }

        [ColumnDisplay(DisplayIndex = 7, Name = "System Uptime (Seconds)", Formatter = Formatters.Interval)]
        public long SysUptimeSeconds { get; set; }

        [ColumnDisplay(DisplayIndex = 8)]
        public DateTimeOffset FirstSeen { get; set; }

        [ColumnDisplay(DisplayIndex = 9)]
        public DateTimeOffset LastSeen { get; set; }
    }
}
