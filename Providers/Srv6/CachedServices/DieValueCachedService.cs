using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using VueExample.Cache.Redis;
using VueExample.Entities;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.Services;
using VueExample.Services.Abstract;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6.CachedServices
{
    public class DieValueCachedService : IDieValueService
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly DieValueService _dieValueService;
        private readonly IGraphic4Service _graphic4Service;
        public DieValueCachedService(ICacheProvider cacheProvider, DieValueService dieValueService, IGraphic4Service graphic4Service)
        {
            _cacheProvider = cacheProvider;
            _dieValueService = dieValueService;
            _graphic4Service = graphic4Service;
        }
        public async Task CreateDieGraphics(List<DieGraphics> dieGraphics)
        {
            await _dieValueService.CreateDieGraphics(dieGraphics);
        }

        public async Task<Dictionary<string, List<DieValue>>> GetDieValuesByMeasurementRecording(int measurementRecordingId)
        {
            var dieValueDictionary = await _cacheProvider.GetFromCache<Dictionary<string, List<DieValue>>>($"DIEVALUE:MEASUREMENTRECORDINGID:{measurementRecordingId}");
            if(dieValueDictionary is null) 
            {
                dieValueDictionary = await _dieValueService.GetDieValuesByMeasurementRecording(measurementRecordingId);
                if(dieValueDictionary.Count == 0)
                {
                    var graphic4ViewModel = await _graphic4Service.GetGraphic4ByMeasurementRecordingId(measurementRecordingId);
                    if(graphic4ViewModel is null)
                    {
                        return new Dictionary<string, List<DieValue>>();
                    }
                    dieValueDictionary = ConvertFromGraphic4ViewModel(graphic4ViewModel);
                }
                await _cacheProvider.SetCache<Dictionary<string, List<DieValue>>>($"DIEVALUE:MEASUREMENTRECORDINGID:{measurementRecordingId}", dieValueDictionary, new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(30)));
            }
            return dieValueDictionary;
        }

        public async Task<List<DieValue>> GetDieValuesByMeasurementRecordingAndKeyGraphicState(int measurementRecordingId, string keyGraphicState)
        {
            var dieValues = await _cacheProvider.GetFromCache<List<DieValue>>($"DIEVALUE:MEASUREMENTRECORDINGID:{measurementRecordingId}:KGS:{keyGraphicState}");
            if(dieValues is null) 
            {
                dieValues = await _dieValueService.GetDieValuesByMeasurementRecordingAndKeyGraphicState(measurementRecordingId, keyGraphicState);
                await _cacheProvider.SetCache<List<DieValue>>($"DIEVALUE:MEASUREMENTRECORDINGID:{measurementRecordingId}:KGS:{keyGraphicState}", dieValues, new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(30)));
            }
            return dieValues;
        }

        public async Task<List<long?>> GetSelectedDiesByMeasurementRecordingId(int measurementRecordingId)
        {
            return await _dieValueService.GetSelectedDiesByMeasurementRecordingId(measurementRecordingId);
        }

        private Dictionary<string, List<DieValue>> ConvertFromGraphic4ViewModel(Graphic4ViewModel graphic4ViewModel)
        {
            var dieValueDictionary = new Dictionary<string, List<DieValue>>();
            foreach (var graphicData in graphic4ViewModel.GraphicData)
            {
                foreach (var state in graphicData.States)
                {
                    var dieValueList = new List<DieValue>();
                    foreach (var dieWithCode in graphicData.DieWithCodesList)
                    {
                        var currentState = dieWithCode.ValueListWithState.FirstOrDefault(x => x.State == state);
                        var dieValue = new DieValue();
                        dieValue.DieId = dieWithCode.DieId;
                        dieValue.MeasurementRecordingId = graphic4ViewModel.MeasurementRecordingId;
                        dieValue.GraphicId = graphicData.Graphic.GraphicSRV6Id;
                        dieValue.State = currentState.State;
                        dieValue.XList = dieWithCode.AbscissList;
                        dieValue.YList = currentState.ValueList;
                        dieValueList.Add(dieValue);
                    }
                    dieValueDictionary.Add($"{graphicData.Graphic.GraphicSRV6Id}_{state}", dieValueList);
                }
            }
            return dieValueDictionary;
        }
    }
}