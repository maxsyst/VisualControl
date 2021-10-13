using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using VueExample.Cache.Redis;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.StatisticsCoreRework.Abstract;
using VueExample.StatisticsCoreRework.Models;
using VueExample.StatisticsCoreRework.Services;

namespace VueExample.StatisticsCoreRework.CachedServices
{
    public class StatisticCachedService : IStatisticService
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly StatisticService _statisticService;
        private readonly IStatisticCalculationService _statisticCalculationService;
        private readonly IDieValueService _dieValueService;
        public StatisticCachedService(ICacheProvider cacheProvider, 
                                      StatisticService statisticService, 
                                      IStatisticCalculationService statisticCalculationService, 
                                      IDieValueService dieValueService) 
            => (_cacheProvider, _statisticService, _statisticCalculationService, _dieValueService) 
                = (cacheProvider, statisticService, statisticCalculationService, dieValueService);

        public async Task<Dictionary<string, SingleParameterStatisticCalculated>> GetCalculatedStatisticByMeasurementRecordingGraphicStateAndDies(int measurementRecordingId, string keyGraphicState, double divider, List<long> dieIdList)
        {
            var graphicId = keyGraphicState.Split('_')[0];
            var statParameterDict = ((await GetSingleParameterStatisticByMeasurementRecording(measurementRecordingId)).Where(x => x.Key.Split('_')[0] == graphicId))
                                    .Select(x => x.Value)
                                    .SelectMany(d => d)
                                    .ToDictionary(e => e.Key, e => e.Value);
            return CreateSingleParameterStatisticCalculated(statParameterDict, divider, dieIdList);
        }
        

        public async Task<Dictionary<string, Dictionary<string, SingleParameterStatisticValues>>> GetSingleParameterStatisticByMeasurementRecording(int measurementRecordingId)
        {
            var dict = await _cacheProvider.GetFromCache<Dictionary<string, Dictionary<string, SingleParameterStatisticValues>>>($"SPS:MEASUREMENTRECORDINGID:{measurementRecordingId}");
            if(dict is null) 
            {
                var dieValues = await _dieValueService.GetDieValuesByMeasurementRecording(measurementRecordingId);
                dict = await _statisticService.GetSingleParameterStatisticByDieValues(new ConcurrentDictionary<string, List<DieValue>>(dieValues), measurementRecordingId);
                await _cacheProvider.SetCache<Dictionary<string, Dictionary<string, SingleParameterStatisticValues>>>($"SPS:MEASUREMENTRECORDINGID:{measurementRecordingId}", dict, new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(30)));
            }
            return dict;
        }

        private Dictionary<string, SingleParameterStatisticCalculated> CreateSingleParameterStatisticCalculated(Dictionary<string, SingleParameterStatisticValues> statParameterDict, double divider, List<long> dieIdList) 
        {
            if(dieIdList.Count == 0) 
            {
                return CreateDefaultSingleParameterStatisticCalculated(statParameterDict);
            }
            var dieIdHashSet = new HashSet<long>(dieIdList);
            var dict = new Dictionary<string, SingleParameterStatisticCalculated>();
            foreach (var stat in statParameterDict)
            {
                stat.Value.DieStatDictionary = stat.Value.DieStatDictionary.Where(x => dieIdHashSet.Contains(x.Key)).ToDictionary(kv => kv.Key, kv => kv.Value);
                dict.Add(stat.Key, _statisticCalculationService.Calculate(stat.Value, divider));
            }
            return dict;
        }

        private Dictionary<string, SingleParameterStatisticCalculated> CreateDefaultSingleParameterStatisticCalculated(Dictionary<string, SingleParameterStatisticValues> statParameterDict) 
        {
            var dict = new Dictionary<string, SingleParameterStatisticCalculated>();
            foreach (var stat in statParameterDict)
            {
                dict.Add(stat.Key, new SingleParameterStatisticCalculated(statisticsName: stat.Value.StatisticName, shortStatisticsName: $"{stat.Value.StatisticName.Split(' ').FirstOrDefault()}", unit: stat.Value.Unit));
            }
            return dict;
        }
    }
}