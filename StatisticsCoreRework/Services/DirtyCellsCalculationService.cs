using System.Threading.Tasks;
using VueExample.StatisticsCoreRework.Abstract;
using VueExample.StatisticsCoreRework.Models;

namespace VueExample.StatisticsCoreRework.Services
{
    public class DirtyCellsCalculationService : IDirtyCellsCalculationService
    {

        private readonly IDirtyCellsCalculationFxdService _dirtyCellsCalculationFxdService;
        private readonly IDirtyCellsCalculationStatService _dirtyCellsCalculationStatService;
        public DirtyCellsCalculationService(IDirtyCellsCalculationFxdService dirtyCellsFxdService, IDirtyCellsCalculationStatService dirtyCellsStatService)
        {
            _dirtyCellsCalculationFxdService = dirtyCellsFxdService;
            _dirtyCellsCalculationStatService = dirtyCellsStatService;
        }
        public Task<DirtyCellsShort> CalculateExtended()
        {
            throw new System.NotImplementedException();
        }

        public async Task<DirtyCellsShort> CalculateShort(int measurementRecordingId, string keyGraphicState, DirtyCellsProfile dirtyCellsProfile, SingleParameterStatisticValues singleParameterStatisticValues)
        {
            if(dirtyCellsProfile.Type == "FXD")
            {
                return await _dirtyCellsCalculationFxdService.CalculateShort(measurementRecordingId, keyGraphicState, 
                                                                             dirtyCellsProfile.LowBorder, dirtyCellsProfile.TopBorder, singleParameterStatisticValues);
            }
            if(dirtyCellsProfile.Type == "STAT")
            {
                return await _dirtyCellsCalculationStatService.CalculateShort(measurementRecordingId, keyGraphicState, dirtyCellsProfile.K, singleParameterStatisticValues);
            }
            return new DirtyCellsShort();
        }
    }
}