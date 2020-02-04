using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.ChartModels.ChartJs;
using VueExample.Models.SRV6;

namespace VueExample.Providers
{
    public interface IChartJSProvider
    {
        AbstractChart GetLinearFromDieValues (List<DieValue> dieValuesList, List<long?> dieIdList, double divider);
        Task<AbstractChart> GetHistogramFromDieValues (List<DieValue> dieValuesList, List<long?> dieIdList, double divider);

    }
}