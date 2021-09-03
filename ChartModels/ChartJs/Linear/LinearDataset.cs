using System.Collections.Generic;

namespace VueExample.ChartModels.ChartJs.Linear
{
    public class LinearDataset : Dataset
    {
        public List<double> Data { get; set; } = new List<double>();
        public long DieId { get; set; }
        public string BorderColor { get; set; } 
    }
}