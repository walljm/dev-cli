using System;
using System.Numerics;

namespace ITPIE.CLI.Models
{
    public class RouteSearchResult : SearchResult
    {
        public string RoutingInstance { get; set; }

        public string RouteSource { get; set; }

        public string Prefix { get; set; }

        public string PreferenceMetric { get; set; }

        public BigInteger LocalPreference { get; set; }

        public BigInteger Med { get; set; }

        public string NexthopInet { get; set; }

        public string NexthopInterface { get; set; }

        public string RouteASPath { get; set; }

        public DateTimeOffset SeenFrom { get; set; }

        public DateTimeOffset SeenTo { get; set; }
    }
}