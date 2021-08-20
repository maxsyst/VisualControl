using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.StatisticsCoreRework.Abstract;
using VueExample.StatisticsCoreRework.Models;

namespace VueExample.StatisticsCoreRework.Services
{
    public class DirtyCellsService
    {
        private readonly IStatisticService _statisticService;
        private readonly IDirtyCellsCalculationService _dirtyCellsCalculationService;
        private readonly IDieValueService _dieValueService;
        public DirtyCellsService(IStatisticService statisticService, IDieValueService dieValueService, IDirtyCellsCalculationService dirtyCellsCalculationService)
        {
            _dieValueService = dieValueService;
            _statisticService = statisticService;
            _dirtyCellsCalculationService = dirtyCellsCalculationService;
        }

        public async Task<MeasurementRecordingDirtyCellsSnapshot> GetDirtyCellsInitialSnapShotByMeasurementRecordingId(int measurementRecordingId)
        {
            var diesCount = (await _dieValueService.GetSelectedDiesByMeasurementRecordingId(measurementRecordingId)).Count();
            var measurementRecordingDirtyCellsSnapshot = new MeasurementRecordingDirtyCellsSnapshot();
            measurementRecordingDirtyCellsSnapshot.MeasurementRecordingId = measurementRecordingId;
            var singleStatDictionary = await _statisticService.GetSingleParameterStatisticByMeasurementRecording(measurementRecordingId);
            foreach (var kgs in singleStatDictionary)
            {
                var kgsDict = singleStatDictionary[kgs.Key];
                var profiles = kgsDict.Select(kv => new DirtyCellsProfile {StatName = kv.Value.StatisticName, Type = "STAT", K = "1.5"}).ToList();
                var dc = await GetDirtyCellsShortsByKeyGraphicStateFromSingleParameterStatistic(measurementRecordingId, kgs.Key, kgsDict, profiles);
                var badDies = new HashSet<long>(dc.SelectMany(kv => kv.Value.BadDirtyCells)).ToList();
                var singleGraphicDirtyCells = new SingleGraphicDirtyCells {KeyGraphicState = kgs.Key, 
                                                                           BadDies = badDies, 
                                                                           GoodDiesPercentage = Convert.ToString(Math.Ceiling((1.0 - badDies.Count / (diesCount + 0.0)) * 100), CultureInfo.InvariantCulture),
                                                                           StatNameDirtyCellsDictionary = dc};
                measurementRecordingDirtyCellsSnapshot.SingleGraphicDirtyCellsDictionary.Add(kgs.Key, singleGraphicDirtyCells); 
            }
            measurementRecordingDirtyCellsSnapshot.BadDies = measurementRecordingDirtyCellsSnapshot.SingleGraphicDirtyCellsDictionary.SelectMany(kv => kv.Value.BadDies).ToList();
            measurementRecordingDirtyCellsSnapshot.GoodDiesPercentage = Convert.ToString(Math.Ceiling((1.0 - measurementRecordingDirtyCellsSnapshot.BadDies.Count / (diesCount + 0.0)) * 100), CultureInfo.InvariantCulture);
            return measurementRecordingDirtyCellsSnapshot;
        }

        public async Task<Dictionary<string, DirtyCellsShort>> GetDirtyCellsShortsByKeyGraphicState(int measurementRecordingId, string keyGraphicState, List<DirtyCellsProfile> dirtyCellsProfiles) 
        {
            var singleStatDictionary = await _statisticService.GetSingleParameterStatisticByMeasurementRecording(measurementRecordingId);
            return await GetDirtyCellsShortsByKeyGraphicStateFromSingleParameterStatistic(measurementRecordingId, keyGraphicState, singleStatDictionary[keyGraphicState], dirtyCellsProfiles);
        }

        private async Task<Dictionary<string, DirtyCellsShort>> GetDirtyCellsShortsByKeyGraphicStateFromSingleParameterStatistic(int measurementRecordingId, string keyGraphicState, Dictionary<string, SingleParameterStatisticValues> kgsDictionary, List<DirtyCellsProfile> dirtyCellsProfiles) 
        {
            var dict = new Dictionary<string, DirtyCellsShort>();
            foreach (var dirtyCellsProfile in dirtyCellsProfiles)
            {
                dict.Add(dirtyCellsProfile.StatName, await _dirtyCellsCalculationService.CalculateShort(measurementRecordingId, keyGraphicState, dirtyCellsProfile, kgsDictionary[dirtyCellsProfile.StatName]));
            }
            return dict;
        }

    }
}