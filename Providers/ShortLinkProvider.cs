using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VueExample.Contexts;
using VueExample.Entities;
using VueExample.Models.SRV6;
using VueExample.Providers.Abstract;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ResponseObjects;
using VueExample.StatisticsCoreRework.Abstract;
using VueExample.ViewModels;

namespace VueExample.Providers
{
    public class ShortLinkProvider : IShortLinkProvider
    {
        private readonly Srv6Context _srv6Context;
        private readonly IMapper _mapper;
        private readonly IExportProvider _exportProvider;
        private readonly ISRV6GraphicService  _graphicService;
        private readonly IMeasurementRecordingService _measurementRecordingService;
        public ShortLinkProvider(Srv6Context srv6Context, IMapper mapper, IExportProvider exportProvider, ISRV6GraphicService graphicService, IMeasurementRecordingService measurementRecordingService)
        {
            _srv6Context = srv6Context;
            _mapper = mapper;
            _exportProvider = exportProvider;
            _graphicService = graphicService;
            _measurementRecordingService = measurementRecordingService;
        }

        public async Task<ShortLinkEntity> CreateSRV3(ShortLinkGenerateViewModel shortLinkGenerateViewModel)
        {
            var guid = Guid.NewGuid();
            var shortlinkInfo = Obfuscate(JsonConvert.SerializeObject(shortLinkGenerateViewModel));
            var shortLink = new ShortLinkEntity{GeneratedId = guid, Link = shortlinkInfo, ShortLink = "http://srv3.svr.lan/sl/" + guid };
            _srv6Context.ShortLinkEntities.Add(shortLink);
            await _srv6Context.SaveChangesAsync();
            return shortLink;
        }

        public async Task<ShortLinkEntity> CreateSRV6(string fullLink)
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
                shortLinkViewModel.StatisticNameList = (await _exportProvider.GetStatisticsNameByMeasurementId(shortLinkViewModel.MeasurementRecordingId)).ToList();
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
            if(shorturl.ShortLink.ElementAt(10) == '3')
            {
                return await Srv3Handler(shorturl);
            }
            else
            {
                return await Srv6Handler(shorturl);
            }
        }

        private async Task<ShortLink> Srv6Handler(ShortLinkEntity shorturl)
        {
            var link = shorturl.Link;
            var measurementRecordingId = Convert.ToInt32(link.Split(new string[] {"idmrpcm="}, StringSplitOptions.None)[1].Split('&')[0]);
            var measurementRecording = await _measurementRecordingService.GetById(measurementRecordingId);
            var selectedGraphic = (await _srv6Context.FkMrGraphics.Where(x => x.MeasurementRecordingId == measurementRecordingId).ToListAsync()).ElementAtOrDefault(Convert.ToInt32(link.Split(new string[] {"rddl="}, StringSplitOptions.None)[1].Split('&')[0])).GraphicId;
            var selectedDies = Deobfuscate(link.Split(new string[] {"dies="}, StringSplitOptions.None)[1].Split('&')[0]).Split('x').Select(x => Convert.ToInt64(x)).ToList();
            var shortinfo =
                new ShortLink
                    {
                        WaferId = link.Split(new string[] {"wid="}, StringSplitOptions.None)[1].Split('&')[0],
                        Divider = (await _srv6Context.Dividers.ToListAsync()).ElementAtOrDefault(Convert.ToInt32(link.Split(new string[] {"square="}, StringSplitOptions.None)[1].Split('&')[0])),
                        SelectedDies = selectedDies,
                        SelectedGraphics = selectedGraphic == null ? new List<GraphicShortLinkViewModel>() : new List<GraphicShortLinkViewModel>{new GraphicShortLinkViewModel(await _graphicService.GetById((int)selectedGraphic))},
                        MeasurementRecording = measurementRecording,
                        GeneratedId = shorturl.GeneratedId
                    };
            return shortinfo;
        }

        private async Task<ShortLink> Srv3Handler(ShortLinkEntity shortLinkEntity)
        {
            var generateViewModel = JsonConvert.DeserializeObject<ShortLinkGenerateViewModel>(Deobfuscate(shortLinkEntity.Link));
            var measurementRecording = await _measurementRecordingService.GetById(generateViewModel.MeasurementRecordingId);
            var shortinfo =
                new ShortLink
                    {
                        WaferId = generateViewModel.WaferId,
                        Divider = await _srv6Context.Dividers.FirstOrDefaultAsync(x => x.DividerK == generateViewModel.Divider),
                        SelectedDies = generateViewModel.SelectedDies,
                        SelectedGraphics = generateViewModel.SelectedGraphics,
                        MeasurementRecording = measurementRecording,
                        GeneratedId = shortLinkEntity.GeneratedId
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