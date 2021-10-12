using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LazyCache;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Providers.Srv6
{
    public class GraphicService : ISRV6GraphicService
    {
        private readonly IGraphicProvider _graphicProvider;
        private readonly IAppCache _appCache;
        public GraphicService(IAppCache appCache, IGraphicProvider graphicProvider)
        {
            _appCache = appCache;
            _graphicProvider = graphicProvider;
        }

        public async Task<Graphic> Create(Graphic graphic, int codeProductId)
        {
            return await _graphicProvider.Create(graphic, codeProductId);
        }

        public async Task<Graphic> CreateS2P(int codeProductId, string type)
        {
            var graphic = new Graphic{ Absciss = "Freq", AbscissUnit = "ГГц", Ordinate = type, OrdinateUnit = "дБ", Name = $"{type}/Freq", Type = 1 };
            return await _graphicProvider.Create(graphic, codeProductId);
        }

        public async Task<List<Graphic>> GetByCodeProduct(int codeProductId)
        {
            return await _graphicProvider.GetByCodeProduct(codeProductId);
        }

        public async Task<Graphic> GetByCodeProductAndName(int codeProductId, string name) 
        {
            return await _graphicProvider.GetByCodeProductAndName(codeProductId, name);
        }

        public async Task<Graphic> GetById(int graphicId)
        {
            Func<Task<Graphic>> cachedService = async () => await _graphicProvider.GetById(graphicId);
            return await _appCache.GetOrAddAsync($"GRAPHIC:ID:{graphicId}", cachedService);
        }

        public async Task<List<Graphic>> GetByMeasurementRecordingId(int measurementRecordingId)
        {
            return await _graphicProvider.GetByMeasurementRecordingId(measurementRecordingId);
        }

        public async Task<Graphic> GetGraphicByKeyGraphicState(string keyGraphicState)
        {
            var graphicId = Convert.ToInt32(keyGraphicState.Split('_')[0]);
            Func<Task<Graphic>> cachedService = async () => await _graphicProvider.GetById(graphicId);
            return await _appCache.GetOrAddAsync($"GRAPHIC:KGS:{graphicId}", cachedService);
        }

    }
}
