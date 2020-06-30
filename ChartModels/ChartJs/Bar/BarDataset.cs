using System.Collections.Generic;

namespace VueExample.ChartModels.ChartJs.Bar
{
    public class BarDataset : Dataset
    {
        public List<string> BackgroundColor { get; set; } = new List<string>();
        public List<long?> DieIdList { get; set; }
    }
}