using System.Collections.Generic;

namespace VueExample.StatisticsCore.DirtyCellsCore
{
    public class Limitation
    {
        public string LowBorder { get; set; }
        public string TopBorder { get; set; }
        public List<SingleDirtyCell> SingleDirtyCellsList { get; set; }
        public Limitation()
        {
            SingleDirtyCellsList = new List<SingleDirtyCell>();
        }

    }
}