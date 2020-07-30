using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Entities;
using VueExample.Models.SRV6;
using VueExample.Providers.Abstract;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ResponseObjects;
using VueExample.StatisticsCore.Abstract;
using VueExample.ViewModels;

namespace VueExample.Providers
{
    public class ShortLinkProvider : IShortLinkProvider
    {   
        private readonly Srv6Context _srv6Context;
        private readonly IMapper _mapper;
        private readonly IExportProvider _exportProvider;
        private readonly ISRV6GraphicService  _graphicService;
        public ShortLinkProvider(Srv6Context srv6Context, IMapper mapper, IExportProvider exportProvider, ISRV6GraphicService graphicService)
        {
            _srv6Context = srv6Context;
            _mapper = mapper;
            _exportProvider = exportProvider;
            _graphicService = graphicService;
        }

        public async Task<ShortLinkEntity> Create(string fullLink)
        {
            var guid = Guid.NewGuid();
            var shortLink = new ShortLinkEntity{GeneratedId = guid, Link = fullLink, ShortLink = "http://srv6.svr.lan/FLink.aspx?l=" + guid };
            _srv6Context.ShortLinkEntities.Add(shortLink);
            await _srv6Context.SaveChangesAsync();
            return shortLink;
        }

        public async Task<ShortLink> GetByGeneratedId(string generatedId)
        {
            return await GetFullUrlFromShortUrl(generatedId);
        }

        public async Task<AfterDbManipulationObject<ShortLinkInfoViewModel>> GetElementExportDetails(string shortLink)
        {
            var shortLinkViewModel = _mapper.Map<ShortLinkInfoViewModel>(await GetFullUrlFromShortUrl(shortLink));        
            var obj = new AfterDbManipulationObject<ShortLinkInfoViewModel>();
            if(shortLinkViewModel.GeneratedId == default(Guid))
            {
                obj.AddError(new Error("Ссылка не найдена"));
            }
            if(!obj.HasErrors)
            {
                 shortLinkViewModel.StatisticNameList = (await _exportProvider.GetStatisticsNameByMeasurementId(shortLinkViewModel.MeasurementRecordingId, 1.5)).ToList();
                 obj.SetObject(shortLinkViewModel);
            } 
            if(shortLinkViewModel.StatisticNameList is null || shortLinkViewModel.StatisticNameList.Count == 0)
            {
                obj.AddError(new Error("Статистика не найдена"));
            }              
            return obj;
        }

        private async Task<ShortLink> GetFullUrlFromShortUrl(string shortlink)
        {
            Guid generatedId;
            bool isValidGuid = Guid.TryParse(shortlink, out generatedId);
            if(!isValidGuid)
                return new ShortLink();
            var shorturl =  await _srv6Context.ShortLinkEntities.FirstOrDefaultAsync(x => x.GeneratedId == generatedId);
            if (shorturl == null)
            {
                return new ShortLink();
            }
            var link = shorturl.Link;
            var measurementRecordingId = Convert.ToInt32(link.Split(new string[] {"idmrpcm="}, StringSplitOptions.None)[1].Split('&')[0]);
            var selectedGraphic = _srv6Context.FkMrGraphics.Where(x => x.MeasurementRecordingId == measurementRecordingId).ToList().ElementAtOrDefault(Convert.ToInt32(link.Split(new string[] {"rddl="}, StringSplitOptions.None)[1].Split('&')[0])).GraphicId;
            var selectedDies = Deobfuscate(link.Split(new string[] {"dies="}, StringSplitOptions.None)[1].Split('&')[0]).Split('x').Select(x => Convert.ToInt64(x)).ToList();
            var shortinfo =
                new ShortLink
                    {
                        WaferId = link.Split(new string[] {"wid="}, StringSplitOptions.None)[1].Split('&')[0],
                        Divider = (await _srv6Context.Dividers.ToListAsync()).ElementAtOrDefault(Convert.ToInt32(link.Split(new string[] {"square="}, StringSplitOptions.None)[1].Split('&')[0])),
                        SelectedDies = selectedDies,
                        SelectedGraphics = selectedGraphic == null ? new List<GraphicShortLinkViewModel>() : new List<GraphicShortLinkViewModel>{new GraphicShortLinkViewModel(await _graphicService.GetById((int)selectedGraphic))},
                        MeasurementRecordingId = measurementRecordingId, 
                        GeneratedId = shorturl.GeneratedId
                    };
            return shortinfo;
        }

        public string Obfuscate(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return String.Empty;
            }
            else
            {
                var bytes = Encoding.UTF8.GetBytes(str);
                for (int i = 0; i < bytes.Length; i++) bytes[i] ^= 0x5a;
                return Convert.ToBase64String(bytes);
            }
        }
        public string Deobfuscate(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return String.Empty;
            }
            else
            {
                var bytes = Convert.FromBase64String(str);
                for (int i = 0; i < bytes.Length; i++) bytes[i] ^= 0x5a;
                return Encoding.UTF8.GetString(bytes);
            }            
        }

       
    }
}