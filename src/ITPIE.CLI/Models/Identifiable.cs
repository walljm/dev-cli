namespace ITPIE.CLI.Models
{
    public class Identifiable
    {
        [ColumnDisplay(DisplayIndex = 0, Display = false)]
        public int DeviceId { get; set; }
    }
}
