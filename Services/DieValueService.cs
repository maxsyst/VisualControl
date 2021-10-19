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
            var dieGraphicsList = await _srv6Context.DieGraphics.AsNoTracking().Where(x => x.MeasurementRecordingId == measurementRecordingId).ToListAsync();
            if(dieGraphicsList.Count == 0)
            {
                return new Dictionary<string, List<DieValue>>();
            }
            var dgDictionary = dieGraphicsList.GroupBy(x => x.GraphicId).ToDictionary(x => x.Key, x => x.ToList());
            var typeList = _srv6Context.Graphics.Where(x => dgDictionary.Keys.Contains(x.Id)).ToList();
            var mappedDictionary = (DieGraphicsMappingParallel(dgDictionary, typeList)).OrderBy(x =>  Convert.ToInt32(x.Key.Split('_')[0])).ToDictionary(entry => entry.Key, entry => entry.Value);
            return mappedDictionary;
        }

        public async Task<List<DieValue>> GetDieValuesByMeasurementRecordingAndKeyGraphicState(int measurementRecordingId, string keyGraphicState)
        {
            var graphicId = Convert.ToInt32(keyGraphicState.Split('_')[0]);
            var graphicType = keyGraphicState.Split('_')[1];
            var dieGraphicsList = await _srv6Context.DieGraphics.AsNoTracking().Where(x => x.MeasurementRecordingId == measurementRecordingId && x.GraphicId == graphicId).ToListAsync();
            var dieValues = dieGraphicsList.Select(dieGraphic => (SelectGraphicSrv6ParsingStrategy(GetTypeFromGraphicState(graphicType))).ParseStringGraphic(dieGraphic)).ToList();
            return dieValues;
        }

        public async Task<List<long?>> GetSelectedDiesByMeasurementRecordingId(int measurementRecordingId)
        {
            return await _srv6Context.DiesParameterOld.Where(x => x.MeasurementRecordingId == measurementRecordingId).Select(x => x.DieId).ToListAsync();
        }

        private short GetTypeFromGraphicState(string keyGraphicState)
        {
            if(keyGraphicState == "LNR")
            {
                return 1;
            }
            if(keyGraphicState == "HSTG")
            {
                return 2;
            }
            return 1;
        }

        private ConcurrentDictionary<string, List<DieValue>> DieGraphicsMappingParallel(Dictionary<int, List<DieGraphics>> dieGraphicsDictionary, List<Graphic> graphicsList)
        {
            var dieValueDictionary = new ConcurrentDictionary<string, List<DieValue>>();
            Parallel.ForEach(dieGraphicsDictionary, dieGraphicList =>
            {
                var dieValueList = new List<DieValue>();
                foreach(var dieGraphic in dieGraphicList.Value)
                {
                    var type = graphicsList.FirstOrDefault(x => x.Id == dieGraphic.GraphicId).Type;
                    var afterParseDieValue = (SelectGraphicSrv6ParsingStrategy(type)).ParseStringGraphic(dieGraphic);
                    dieValueList.Add(afterParseDieValue);
                }
                if(dieValueList.Count > 0)
                {
                    dieValueDictionary.TryAdd($"{dieGraphicList.Key}_{dieValueList.FirstOrDefault().State}", dieValueList);
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
