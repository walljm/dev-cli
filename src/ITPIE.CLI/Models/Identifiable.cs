using ITPIE.CLI.Commands;

namespace ITPIE.CLI.Models
{
    public class Identifiable
    {
        [ColumnDisplay(DisplayIndex = 0)]
        public int DeviceId { get; set; }
    }
}
