using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Models.SRV6;
using VueExample.StatisticsCoreRework.Models;

namespace VueExample.StatisticsCoreRework.Abstract
{
    public interface ISingleParameterStatisticService
    {
      //  public abstract List<VueExample.StatisticsCore.DataModels.SingleStatisticData> CreateSingleStatisticData(List<long?> dieList, Graphic graphic, ConcurrentDictionary<long?, DieValue> dieValuesList, double divider, List<VueExample.StatisticsCore.SingleParameterStatistic> singleParameterStatisticsList);
        Task<Dictionary<string, SingleParameterStatisticValues>> CreateSingleParameterStatisticsList(List<DieValue> dieValues, Graphic graphic, string keyGraphicState, int measurementRecordingId);
    }
}