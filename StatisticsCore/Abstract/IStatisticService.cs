using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6;

namespace VueExample.StatisticsCore.Abstract
{
    public interface IStatisticService
    {
        Task<Dictionary<string, List<SingleParameterStatistic>>> GetSingleParameterStatisticByDieValues(ConcurrentDictionary<string, List<DieValue>> dieValues, int? stageId, double divider, double k);
        Task<List<VueExample.StatisticsCore.DataModels.SingleStatisticData>> GetStatisticsDataByGraphicState(List<long?> dieList, string keyGraphicState, List<DieValue> dieValuesList, double divider, List<VueExample.StatisticsCore.SingleParameterStatistic> singleParameterStatisticsList);
        DirtyCells GetDirtyCellsBySPSDictionary(ConcurrentDictionary<string, List<SingleParameterStatistic>> singleParameterStatistics, int diesCount);
    }
}