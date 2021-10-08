namespace VueExample.ChartModels.ChartJs.Bar
{
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