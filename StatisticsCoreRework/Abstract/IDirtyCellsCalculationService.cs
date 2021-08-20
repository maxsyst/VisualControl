using System.Threading.Tasks;
using VueExample.StatisticsCoreRework.Models;

namespace VueExample.StatisticsCoreRework.Abstract
{
    public interface IDirtyCellsCalculationService
    {
        Task<DirtyCellsShort> CalculateShort(int measurementRecordingId, string keyGraphicState, DirtyCellsProfile dirtyCellsProfile, SingleParameterStatisticValues singleParameterStatisticValues);
        Task<DirtyCellsShort> CalculateExtended();
    }
}