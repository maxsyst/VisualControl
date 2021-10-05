using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6;
using VueExample.StatisticsCore.DirtyCellsCore;

namespace VueExample.StatisticsCore.Abstract
{
    public interface IStatisticService
    {
        Task<Dictionary<string, List<SingleParameterStatistic>>> GetSingleParameterStatisticByDieValues(ConcurrentDictionary<string, List<DieValue>> dieValues, int measurementRecordingId, int? stageId, double divider, double k);
        Task<List<VueExample.StatisticsCore.DataModels.SingleStatisticData>> GetStatisticsDataByGraphicState(List<long?> dieList, string keyGraphicState, List<DieValue> dieValuesList, double divider, List<VueExample.StatisticsCore.SingleParameterStatistic> singleParameterStatisticsList);
        DirtyCells GetDirtyCellsBySPSDictionary(ConcurrentDictionary<string, List<SingleParameterStatistic>> singleParameterStatistics, int diesCount);
        List<GraphicDirtyCells> GetGraphicDirtyCells (Dictionary<string, List<SingleParameterStatistic>> singleParameterStatistics, List<KurbatovParameterModel> kurbatovParameterList);
    }
}