using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.Charts;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IControlChartsProvider
    {
        Task<ControlChartsData> GetChartData(List<string> waferList, string square, int stageId, string parameter, string modesd, string modevisual);
    }
}