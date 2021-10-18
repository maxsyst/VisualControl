using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.ChartModels.ChartJs;
using VueExample.Models.SRV6;

namespace VueExample.Providers
{
    public interface IChartJSProvider
    {
        Task<AbstractChart> GetLinearFromDieValues (Dictionary <long?, DieValue> dieValuesDictionary, List<long?> dieIdList, double divider, string keyGraphicState);
        Task<AbstractChart> GetHistogramFromDieValues (List<DieValue> dieValuesList, List<long?> dieIdList, double divider, string keyGraphicState);
    }
}