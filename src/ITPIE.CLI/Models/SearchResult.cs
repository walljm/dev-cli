namespace ITPIE.CLI.Models
{
    public class SearchResult : Identifiable
    {
        [ColumnDisplay(DisplayIndex = 1, Name = "Device IP")]
        public string DeviceIp { get; set; }

        [ColumnDisplay(DisplayIndex = 1)]
        public string Hostname { get; set; }
    }
}
