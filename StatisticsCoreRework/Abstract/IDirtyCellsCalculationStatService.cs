using System.Threading.Tasks;
using VueExample.StatisticsCoreRework.Models;

namespace VueExample.StatisticsCoreRework.Abstract
{
    public interface IDirtyCellsCalculationStatService
    {
        Task<DirtyCellsShort> CalculateShort(int measurementRecordingId, string keyGraphicState, string k, SingleParameterStatisticValues singleParameterStatisticValues);
    }
}