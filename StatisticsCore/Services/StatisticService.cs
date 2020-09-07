using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Contexts;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.StatisticsCore.Services
{
    public class StatisticService 
    {
        private readonly ISRV6GraphicService _graphicService;
        private readonly IServiceProvider _services;
        public StatisticService(ISRV6GraphicService graphicService, IServiceProvider services)
        {
            _services = services;
            _graphicService = graphicService;
        }
        
        public async Task<Dictionary<string, List<SingleParameterStatistic>>> GetSingleParameterStatisticByDieValues(Dictionary<string, List<DieValue>> dieValues, int? stageId, double divider, double k) {
            var statisticsDictionary = new Dictionary<string, List<SingleParameterStatistic>>();
            foreach (var graphicDV in dieValues) 
            {
             
                var graphic = await _graphicService.GetById(Convert.ToInt32(graphicDV.Key.Split('_')[0]));
                var singleParameterStatisticsList = SingleStatisticsServiceCreator(graphic).CreateSingleParameterStatisticsList(graphicDV.Value, graphic, stageId, divider, k);
                statisticsDictionary.Add(graphicDV.Key, singleParameterStatisticsList);
            }
            return statisticsDictionary;
        }

        public async Task<List<VueExample.StatisticsCore.DataModels.SingleStatisticData>> GetStatisticsDataByGraphicState(List<long?> dieList, string keyGraphicState, List<DieValue> dieValuesList, double divider, List<VueExample.StatisticsCore.SingleParameterStatistic> singleParameterStatisticsList) 
        {
            var graphicId = Convert.ToInt32(keyGraphicState.Split('_')[0]);
            var graphic = await _graphicService.GetById(graphicId);  
            var dieValuesDictionaryDieIdBased = new Dictionary<long?, DieValue>();
            foreach (var dieValue in dieValuesList)
            {
                dieValuesDictionaryDieIdBased.Add(dieValue.DieId, dieValue);
            }
            return SingleStatisticsServiceCreator(graphic).CreateSingleStatisticData(dieList, graphic, dieValuesDictionaryDieIdBased, divider, singleParameterStatisticsList);
        }

        public DirtyCells GetDirtyCellsBySPSDictionary (Dictionary<string, List<SingleParameterStatistic>> singleParameterStatistics, int diesCount) 
        {
            var dirtyCells = new DirtyCells();
            foreach (var item in singleParameterStatistics) 
            {
                foreach (var singleParameterStatistic in item.Value) 
                {
                    dirtyCells.StatList.AddRange(singleParameterStatistic.DirtyCells.StatList);
                    dirtyCells.FixedList.AddRange(singleParameterStatistic.DirtyCells.FixedList);
                }
            }
            return dirtyCells.Distinct().CalculatePercentage(diesCount);
        }

       private SingleStatisticServices.Abstract.SingleStatisticsServiceAbstract SingleStatisticsServiceCreator(Graphic graphic)
       {
            var statParameterService = new StatParameterService(_services);
            switch (graphic.Type)
            {
                case 1:
                    return new SingleStatisticServices.SingleParameterServiceLNR(statParameterService);
                case 2:
                    return new SingleStatisticServices.SingleParameterServiceHSTG(statParameterService);
            }
            return new SingleStatisticServices.SingleParameterServiceLNR(statParameterService);;

       }

    }
}