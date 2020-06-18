using System.Collections.Generic;

namespace ITPIE.CLI.Models
{
    public class PagedResponse<T>
    {
        public IList<T> Items { get; set; } = new List<T>();

        public bool HasMore { get { return this.Total > this.Items.Count; } }

        public int? Total { get; set; }

        public int PageSize { get; set; }

        public int Page { get; set; }
    }
}