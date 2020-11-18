using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.Charts;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Providers.Srv6
{
    public class ControlChartsProvider : IControlChartsProvider
    {
        public Task<ControlChartsData> GetChartData(List<string> waferList, string square, int stageId, string parameter, string modesd, string modevisual)
        {
            throw new System.NotImplementedException();
        }
    }
}