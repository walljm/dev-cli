namespace CLI.Commands.Itpie.Models
{
    public class CollectionJobTargetRequest
    {
        public string Protocol { get; set; }
        public string IncludedRanges { get; set; }
        public string ExcludedRanges { get; set; }
    }
}
