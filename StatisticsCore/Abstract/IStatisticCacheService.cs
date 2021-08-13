using System.Collections.Generic;
using System.Threading.Tasks;

namespace VueExample.StatisticsCore.Abstract
{
    public interface IStatisticCacheService
    {
        Task<List<SingleParameterStatistic>> GetSingleParameterStatisticByMeasurementRecordingIdAndKeyGraphicState(int measurementRecordingId, double k, string keyGraphicState);
    }
}