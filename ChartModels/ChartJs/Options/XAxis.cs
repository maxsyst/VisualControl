namespace VueExample.ChartModels.ChartJs.Options
{
    public class XAxis
    {
        public string Label { get; set; }
        public bool Display { get; set; } = true;
        public XAxis()
        {
        }
        public XAxis(string label, bool display)
        {
            this.Label = label;
            this.Display = display;
        }
    }
}