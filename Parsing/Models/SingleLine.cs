using System.Collections.Generic;

namespace VueExample.Parsing.Models
{
    public class SingleLine
    {
        public List<string> AbscissList { get; set; } = new List<string>();
        public List<string> ValueList { get; set; } = new List<string>();
        public SingleLine() 
        {

        }
        protected SingleLine(SingleLine singleLine)
        {
            this.AbscissList = singleLine.AbscissList;
            this.ValueList = singleLine.ValueList;
        }
        
    }
}