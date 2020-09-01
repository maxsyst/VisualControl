using VueExample.ChartModels.ChartJs.Options;

namespace VueExample.ChartModels.ChartJs.Linear
{
    public class LinearOptions : VueExample.ChartModels.ChartJs.Abstract.Options
    {
        public LinearOptions(XAxis xAxis, YAxis yAxis)
        {
            Legend = new ChartJs.Options.Legend();
            Tooltips = new ChartJs.Options.Tooltips();
            Elements = new ChartJs.Options.Elements();
            XAxis = xAxis;
            YAxis = yAxis;
        }

        public ChartJs.Options.Legend Legend { get; set; }
        public ChartJs.Options.Tooltips Tooltips { get; set; }
        public ChartJs.Options.Elements Elements { get; set; }
        public ChartJs.Options.XAxis XAxis { get; set; }
        public ChartJs.Options.YAxis YAxis { get; set; }
    }
}