using System.Collections.Generic;
using VueExample.ChartModels.ChartJs.Options;

namespace VueExample.ChartModels.ChartJs.Bar
{
    public class BarChart : AbstractChart
    {
        public BarChart(List<string> labelsList, Dictionary<long, Dataset> dataSets, XAxis xAxis, YAxis yAxis)
        {
            this.Options = new BarOptions(xAxis, yAxis);
            this.ChartData = new ChartData(labelsList, dataSets);
        }
    }
}