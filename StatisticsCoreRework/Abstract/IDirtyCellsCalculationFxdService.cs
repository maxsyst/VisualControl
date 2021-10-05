using System.Threading.Tasks;
using VueExample.StatisticsCoreRework.Models;

namespace VueExample.StatisticsCoreRework.Abstract
{
    public interface IDirtyCellsCalculationFxdService
    {
        Task<DirtyCellsShort> CalculateShort(int measurementRecordingId, string keyGraphicState, string lowBorder, string topBorder, SingleParameterStatisticValues singleParameterStatisticValues);
    }
}