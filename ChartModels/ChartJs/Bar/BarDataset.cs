using System.Collections.Generic;

namespace VueExample.ChartModels.ChartJs.Bar
{
    public class BarDataset : Dataset
    {
        public List<string> BackgroundColor { get; set; } = new List<string>();
        public Dictionary<long, double> Data { get; set; } = new Dictionary<long, double>();
    }

    public class SingleBarDataset
    {
        public long DieId { get; set; }
        public string BackgroundColor { get; set; }
        public double Value { get; set; }

        public SingleBarDataset(long dieId, string backgroundColor, double value)
        {
            DieId = dieId;
            BackgroundColor = backgroundColor;
            Value = value;
        }
    }
}