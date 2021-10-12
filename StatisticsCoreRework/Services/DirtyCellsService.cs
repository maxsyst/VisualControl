using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VueExample.Enums;
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
        private readonly ISRV6GraphicService _graphicService;
        public DirtyCellsService(IStatisticService statisticService, ISRV6GraphicService graphicService, IDieValueService dieValueService, IDirtyCellsCalculationService dirtyCellsCalculationService)
        {
            _dieValueService = dieValueService;
            _statisticService = statisticService;
            _dirtyCellsCalculationService = dirtyCellsCalculationService;
            _graphicService = graphicService;
        }

        public async Task<MeasurementRecordingDirtyCellsSnapshot> GetDirtyCellsInitialSnapShotByMeasurementRecordingId(int measurementRecordingId)
        {
            var selectedDies = (await _dieValueService.GetSelectedDiesByMeasurementRecordingId(measurementRecordingId));
            var diesCount = selectedDies.Count();
            var measurementRecordingDirtyCellsSnapshot = new MeasurementRecordingDirtyCellsSnapshot();
            measurementRecordingDirtyCellsSnapshot.MeasurementRecordingId = measurementRecordingId;
            var singleStatDictionary = await _statisticService.GetSingleParameterStatisticByMeasurementRecording(measurementRecordingId);
            var graphicList = await _graphicService.GetByMeasurementRecordingId(measurementRecordingId);
            var dirtyCellsProfiles = new Dictionary<string, List<DirtyCellsProfile>>();
            var singleGraphicDirtyCellsDictionary = new Dictionary<string, SingleGraphicDirtyCells>();
            foreach (var kgs in singleStatDictionary)
            {
                var kgsDict = singleStatDictionary[kgs.Key];
                var profiles = kgsDict.Select(kv => new DirtyCellsProfile { StatName = kv.Value.StatisticName, Type = "STAT", K = "1.5" }).ToList();
                dirtyCellsProfiles.Add(kgs.Key, profiles);
                var dc = await GetDirtyCellsShortsByKeyGraphicStateFromSingleParameterStatistic(measurementRecordingId, kgs.Key, kgsDict, profiles);
                SetProfilesBorders(profiles, dc);
                var badDies = new HashSet<long>(dc.SelectMany(kv => kv.Value.BadDirtyCells)).ToList();
                var singleGraphicDirtyCells = new SingleGraphicDirtyCells
                {
                    KeyGraphicState = kgs.Key,
                    BadDies = badDies,
                    GoodDiesPercentage = Convert.ToString(Math.Ceiling((1.0 - badDies.Count / (diesCount + 0.0)) * 100), CultureInfo.InvariantCulture),
                    StatNameDirtyCellsDictionary = dc
                };
                singleGraphicDirtyCellsDictionary.Add(kgs.Key, singleGraphicDirtyCells);
            }
            foreach (var graphic in graphicList)
            {
                var graphicId = Convert.ToString(graphic.Id);
                var kgs =  $"{graphicId}_{Enum.GetName(typeof(GraphicType), graphic.Type)}";
                measurementRecordingDirtyCellsSnapshot.DirtyCellsProfiles.Add(
                    kgs, 
                    dirtyCellsProfiles.Where(x => x.Key.Split('_')[0] == graphicId).Select(x => x.Value).SelectMany(d => d).ToList()
                );
                var singleGraphicDirtyCellsList = singleGraphicDirtyCellsDictionary.Where(x => x.Key.Split('_')[0] == graphicId).Select(x => x.Value).ToList();
                var badDies = new HashSet<long>(singleGraphicDirtyCellsList.SelectMany(x => x.BadDies)).ToList();
                var singleGraphicDirtyCells = new SingleGraphicDirtyCells {
                    KeyGraphicState = kgs,
                    BadDies = badDies,
                    GoodDiesPercentage = Convert.ToString(Math.Ceiling((1.0 - badDies.Count / (diesCount + 0.0)) * 100), CultureInfo.InvariantCulture),
                    StatNameDirtyCellsDictionary = singleGraphicDirtyCellsList.SelectMany(x => x.StatNameDirtyCellsDictionary).ToDictionary(e => e.Key, e => e.Value)
                };
                measurementRecordingDirtyCellsSnapshot.SingleGraphicDirtyCellsDictionary.Add(kgs, singleGraphicDirtyCells);
            }
            measurementRecordingDirtyCellsSnapshot.SelectedDies = selectedDies.Select(x => (long)x).ToList();
            measurementRecordingDirtyCellsSnapshot.BadDies = new HashSet<long>(measurementRecordingDirtyCellsSnapshot.SingleGraphicDirtyCellsDictionary.SelectMany(kv => kv.Value.BadDies)).ToList();
            measurementRecordingDirtyCellsSnapshot.GoodDiesPercentage = Convert.ToString(Math.Ceiling((1.0 - measurementRecordingDirtyCellsSnapshot.BadDies.Count / (diesCount + 0.0)) * 100), CultureInfo.InvariantCulture);
            return measurementRecordingDirtyCellsSnapshot;
        }

        public async Task<SingleGraphicDirtyCells> GetDirtyCellsShortsByKeyGraphicState(int measurementRecordingId, string keyGraphicState, List<DirtyCellsProfile> dirtyCellsProfiles) 
        {
            var selectedDies = (await _dieValueService.GetSelectedDiesByMeasurementRecordingId(measurementRecordingId));
            var diesCount = selectedDies.Count();
            var singleStatDictionary = await _statisticService.GetSingleParameterStatisticByMeasurementRecording(measurementRecordingId);
            var dc = await GetDirtyCellsShortsByKeyGraphicStateFromSingleParameterStatistic(measurementRecordingId, keyGraphicState, singleStatDictionary[keyGraphicState], dirtyCellsProfiles);
            var badDies = new HashSet<long>(dc.SelectMany(kv => kv.Value.BadDirtyCells)).ToList();
            var singleGraphicDirtyCells = new SingleGraphicDirtyCells
            {
                KeyGraphicState = keyGraphicState,
                BadDies = badDies,
                GoodDiesPercentage = Convert.ToString(Math.Ceiling((1.0 - badDies.Count / (diesCount + 0.0)) * 100), CultureInfo.InvariantCulture),
                StatNameDirtyCellsDictionary = dc
            };
            return singleGraphicDirtyCells;
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

        private void SetProfilesBorders(List<DirtyCellsProfile> profiles, Dictionary<string, DirtyCellsShort> dc)
        {
            foreach (var profile in profiles)
            {
                var dcStat = dc[profile.StatName];
                profile.LowBorder = dcStat.LowBorder;
                profile.TopBorder = dcStat.TopBorder;
            }
        }
    }
}