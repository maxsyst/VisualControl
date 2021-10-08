using System.Collections.Generic;
using System.Linq;
using VueExample.ChartModels.AmChart;
using VueExample.ChartModels.AmChart.Linear;
using VueExample.Models.SRV6;

namespace VueExample.Providers
{
    public class AmChartProvider
    {
        public AmChart<Series> GetLinearFromDieValues(List<DieValue> dieValuesList, List<long?> dieIdList)
        {
            var amchart = new LinearChart();
            foreach (var dieValue in dieValuesList.Where(x => dieIdList.Contains(x.DieId)).ToList())
            {
                var series = new Series();
                for (int i = 0; i < dieValue.XList.Count; i++)
                {
                    var point = new VueExample.ChartModels.AmChart.Linear.Point(dieValue.XList[i], dieValue.YList[i]);
                    series.pointList.Add(point);
                }
                amchart.Data.Add(series);
            }
            return amchart;
        }
    }
}
