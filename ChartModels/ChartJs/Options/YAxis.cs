namespace VueExample.ChartModels.ChartJs.Options
{
    public class YAxis
    {
        public string Label { get; set; }
        public bool Display { get; set; } = true;
        public YAxis()
        {
        }
        public YAxis(string label, bool display)
        {
            this.Label = label;
            this.Display = display;
        }       
    }
}