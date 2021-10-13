using System.Collections.Generic;

namespace VueExample.Parsing.Models
{
    public class ValueListWithState
    {
        public string State { get; set; }
        public List<string> ValueList { get; set; } = new List<string>();
    }
}