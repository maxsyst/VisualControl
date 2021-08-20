using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.StatisticsCoreRework.Abstract;
using VueExample.StatisticsCoreRework.Models;

namespace VueExample.StatisticsCoreRework.Services
{
    public class DirtyCellsService : IDirtyCellsService
    {
        private readonly IStatisticService _statisticService;
        private readonly IDirtyCellsCalculationService _dirtyCellsCalculationService;
        public DirtyCellsService(IStatisticService statisticService, IDirtyCellsCalculationService dirtyCellsCalculationService)
        {
            _statisticService = statisticService;
            _dirtyCellsCalculationService = dirtyCellsCalculationService;
        }

        public async Task<Dictionary<string, DirtyCellsShort>> GetDirtyCellsShortsByKeyGraphicState(int measurementRecordingId, string keyGraphicState, List<DirtyCellsProfile> dirtyCellsProfiles) 
        {
            var dict = new Dictionary<string, DirtyCellsShort>();
            var kgsDict = (await _statisticService.GetSingleParameterStatisticByMeasurementRecording(measurementRecordingId))[keyGraphicState];
            foreach (var dirtyCellsProfile in dirtyCellsProfiles)
            {
                dict.Add(dirtyCellsProfile.StatName, await _dirtyCellsCalculationService.CalculateShort(measurementRecordingId, keyGraphicState, dirtyCellsProfile, kgsDict[dirtyCellsProfile.StatName]));
            }
            return dict;
        }

    }
}