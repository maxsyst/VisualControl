using System.Collections.Generic;

namespace VueExample.StatisticsCoreRework.Models
{
    public class Limitation
    {
        public string LowBorder { get; set; }
        public string TopBorder { get; set; }
        public List<DirtyCellExtended> DirtyCellExtendedList { get; set; }
        public Limitation()
        {
            DirtyCellExtendedList = new List<DirtyCellExtended>();
        }

    }
}