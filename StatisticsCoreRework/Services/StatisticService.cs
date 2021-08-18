using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.StatisticsCoreRework.Abstract;
using VueExample.StatisticsCoreRework.CachedServices;
using VueExample.StatisticsCoreRework.Models;

namespace VueExample.StatisticsCoreRework.Services
{
    public class StatisticService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISRV6GraphicService _graphicService;
       
        public StatisticService(IServiceProvider serviceProvider, ISRV6GraphicService graphicService)
        {
            _serviceProvider = serviceProvider;
            _graphicService = graphicService;
        }
        public async Task<Dictionary<string, Dictionary<string, SingleParameterStatisticValues>>> GetSingleParameterStatisticByDieValues(ConcurrentDictionary<string, List<DieValue>> dieValues, int measurementRecordingId)
        {
            var statisticsDictionary = new Dictionary<string, Dictionary<string, SingleParameterStatisticValues>>();
            foreach (var graphicDV in dieValues)
            {
                var graphic = await _graphicService.GetById(Convert.ToInt32(graphicDV.Key.Split('_')[0]));
                var singleParameterStatisticsList = await SingleStatisticsServiceCreator(graphic).CreateSingleParameterStatisticsList(graphicDV.Value, graphic, graphicDV.Key, measurementRecordingId);
                statisticsDictionary.Add(graphicDV.Key, singleParameterStatisticsList.ToDictionary(x => x.Key, x => x.Value));
            }
            return statisticsDictionary;
        }

        private ISingleParameterStatisticService SingleStatisticsServiceCreator(Graphic graphic)
        {
            var services = _serviceProvider.GetServices<ISingleParameterStatisticService>();
            switch (graphic.Type)
            {
                case 1:
                    return services.First(x => x.GetType() == typeof(SingleParameterCachedServiceLNR));
                case 2:
                    return services.First(x => x.GetType() == typeof(SingleParameterCachedServiceHSTG));
            }
            return services.First(x => x.GetType() == typeof(SingleParameterCachedServiceHSTG));

       }

    }
}