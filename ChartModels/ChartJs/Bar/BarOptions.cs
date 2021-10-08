using VueExample.ChartModels.ChartJs.Options;

namespace VueExample.ChartModels.ChartJs.Bar
{
    public class BarOptions : VueExample.ChartModels.ChartJs.Abstract.Options
    {
        public BarOptions(XAxis xAxis, YAxis yAxis)
        {
            Legend = new ChartJs.Options.Legend();
            XAxis = xAxis;
            YAxis = yAxis;
        }

        public ChartJs.Options.Legend Legend { get; set; }
        public ChartJs.Options.XAxis XAxis { get; set; }
        public ChartJs.Options.YAxis YAxis { get; set; }
    }
}