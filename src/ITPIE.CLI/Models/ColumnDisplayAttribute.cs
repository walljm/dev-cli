using System;

namespace ITPIE.CLI.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnDisplayAttribute : Attribute
    {
        public string Name { get; set; }

        public int DisplayIndex { get; set; } = int.MaxValue;

        public bool Display {get;set;} = true;
    }
}
