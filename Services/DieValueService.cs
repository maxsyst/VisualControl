using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VueExample.Contexts;
using VueExample.Entities;
using VueExample.Models.SRV6;
using VueExample.Parsing.Concrete;
using VueExample.Parsing.Strategies;
using VueExample.Providers.Srv6.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System;

namespace VueExample.Services
{
    public class DieValueService : IDieValueService
    {
        private readonly ISRV6GraphicService _graphicService;
        private readonly Srv6Context _srv6Context;
        public DieValueService(ISRV6GraphicService graphicService, Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
            _graphicService = graphicService;
        }
        public async Task CreateDieGraphics(List<DieGraphics> dieGraphics)
        {
            _srv6Context.AddRange(dieGraphics);
            await _srv6Context.SaveChangesAsync();
        }

        public async Task<Dictionary<string, List<DieValue>>> GetDieValuesByMeasurementRecording(int measurementRecordingId)
        {
            var dieGraphicsList = _srv6Context.DieGraphics.AsNoTracking().Where(x => x.MeasurementRecordingId == measurementRecordingId);
            var dgDictionary = await dieGraphicsList.GroupBy(x => x.GraphicId, x => x).ToDictionaryAsync(x => x.Key, x => x.ToList());
            var typeList = (await Task.WhenAll(dgDictionary.Keys.Select(async x => await _graphicService.GetById(x)))).ToList();
            var mappedDictionary = (DieGraphicsMappingParallel(dgDictionary, typeList)).OrderBy(x => Convert.ToInt32(x.Key.Split('_')[0])).ToDictionary(entry => entry.Key, entry => entry.Value);
            return mappedDictionary;
        }

        public async Task<List<long?>> GetSelectedDiesByMeasurementRecordingId(int measurementRecordingId)
        {
            return await _srv6Context.DiesParameterOld.Where(x => x.MeasurementRecordingId == measurementRecordingId).Select(x => x.DieId).ToListAsync();
        }
       
        private ConcurrentDictionary<string, List<DieValue>> DieGraphicsMappingParallel(Dictionary<int, List<DieGraphics>> dieGraphicsDictionary, List<Graphic> graphicsList)
        {
            var dieValueDictionary = new ConcurrentDictionary<string, List<DieValue>>();
            Parallel.ForEach(dieGraphicsDictionary, dieGraphicList => 
            {
                foreach(var dieGraphic in dieGraphicList.Value)
                {
                    var type = graphicsList.FirstOrDefault(x => x.Id == dieGraphic.GraphicId).Type;
                    var afterParseDictionary = (SelectGraphicSrv6ParsingStrategy(type)).ParseStringGraphic(dieGraphic);
                    foreach (var item in afterParseDictionary)
                    {
                        dieValueDictionary.TryAdd(item.Key, new List<DieValue>());
                        dieValueDictionary[item.Key].Add(item.Value);                            
                    }
                }   
            });
           return dieValueDictionary;
        }               
        
        private IStringGraphicSRV6ParsingStrategy SelectGraphicSrv6ParsingStrategy(int type)
        {
            IStringGraphicSRV6ParsingStrategy stringGraphicSrv6ParsingStrategy = new CommonLinearStringGraphicParsingStrategy();
            if (type is 2) stringGraphicSrv6ParsingStrategy = new HistogramStringGraphicParsingStrategy();
            else if (type is 4) stringGraphicSrv6ParsingStrategy = new G4StringGraphicParsingStrategy();
            return stringGraphicSrv6ParsingStrategy;
        }
    }
}
