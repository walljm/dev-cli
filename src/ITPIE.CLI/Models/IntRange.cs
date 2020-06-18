using System.Collections.Generic;
using Newtonsoft.Json;

namespace ITPIE.CLI.Models
{
    public class IntRange
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? First { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Last { get; set; }

        public override string ToString()
        {
            if (this.First == this.Last)
            {
                return this.First?.ToString() ?? string.Empty;
            }

            return $"{this.First}-{this.Last}";
        }
    }

    public class EnumerableIntRange : List<IntRange>
    {
        public override string ToString()
        {
            return string.Join(", ", this);
        }
    }
}
