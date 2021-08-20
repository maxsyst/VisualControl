using System.Collections.Generic;

namespace VueExample.StatisticsCoreRework.Models
{
    public class DirtyCellsShort
    {
        public string StatName { get; set; }
        public string Type { get; set; }
        public List<long> BadDirtyCells { get; set; } = new List<long>();

        public DirtyCellsShort()
        {
            
        }
        public DirtyCellsShort(string statName)
        {
            StatName = statName;
        }

        public DirtyCellsShort SetBadDies(List<long> badDiesId)
        {
            BadDirtyCells = badDiesId;
            return this;
        }
    }
    
}