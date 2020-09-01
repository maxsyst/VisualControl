using System.Collections.Generic;
using VueExample.ChartModels.ChartJs.Options;

namespace VueExample.ChartModels.ChartJs.Linear
{
    public class LinearChart : AbstractChart
    {
        public LinearChart(List<string> labelsList, IEnumerable<Dataset> dataSetList, XAxis xAxis, YAxis yAxis)
        {
            this.Options = new LinearOptions(xAxis, yAxis);
            this.ChartData = new ChartData(labelsList, dataSetList);
        }
    }
}