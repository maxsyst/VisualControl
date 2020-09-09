using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VueExample.Contexts;
using VueExample.Models.SRV6;
using VueExample.Providers.Srv6;
using VueExample.Providers.Srv6.Interfaces;
using System.Linq;

namespace VueExample.StatisticsCore.Services
{
    public class StatisticService 
    {
        private readonly ISRV6GraphicService _graphicService;
        private readonly IStatParameterService _statisticService;
        public StatisticService(ISRV6GraphicService graphicService, IStatParameterService statisticService)
        {
            _graphicService = graphicService;
            _statisticService = statisticService;
        }
        
        public async Task<Dictionary<string, List<SingleParameterStatistic>>> GetSingleParameterStatisticByDieValues(ConcurrentDictionary<string, List<DieValue>> dieValues, int? stageId, double divider, double k)
        {
            var statisticsDictionary = new Dictionary<string, List<SingleParameterStatistic>>();
            var statParameterStageList = await _statisticService.GetByStageId(stageId);
            foreach (var graphicDV in dieValues)
            {
                var graphic = await _graphicService.GetById(Convert.ToInt32(graphicDV.Key.Split('_')[0]));
                var singleParameterStatisticsList = SingleStatisticsServiceCreator(graphic).CreateSingleParameterStatisticsList(graphicDV.Value, graphic, statParameterStageList, divider, k);
                statisticsDictionary.Add(graphicDV.Key, singleParameterStatisticsList);
            }
            return statisticsDictionary.ToDictionary(x => x.Key, x => x.Value);
        }

        public async Task<List<VueExample.StatisticsCore.DataModels.SingleStatisticData>> GetStatisticsDataByGraphicState(List<long?> dieList, string keyGraphicState, List<DieValue> dieValuesList, double divider, List<VueExample.StatisticsCore.SingleParameterStatistic> singleParameterStatisticsList) 
        {
            var graphicId = Convert.ToInt32(keyGraphicState.Split('_')[0]);
            var graphic = await _graphicService.GetById(graphicId);  
            var dieValuesDictionaryDieIdBased = new ConcurrentDictionary<long?, DieValue>();
            Parallel.ForEach (dieValuesList, dieValue =>
            {
                dieValuesDictionaryDieIdBased.TryAdd(dieValue.DieId, dieValue);
            });
            return SingleStatisticsServiceCreator(graphic).CreateSingleStatisticData(dieList, graphic, dieValuesDictionaryDieIdBased, divider, singleParameterStatisticsList);
        }

        public DirtyCells GetDirtyCellsBySPSDictionary(ConcurrentDictionary<string, List<SingleParameterStatistic>> singleParameterStatistics, int diesCount) 
        {
            var dirtyCells = new DirtyCells();
            var statListCBag = new List<long?>();
            var fixedListCBag = new List<long?>();
            var sync = new object();
            Parallel.ForEach (singleParameterStatistics, item =>  
            {
                foreach (var singleParameterStatistic in item.Value)
                {
                    lock (sync) {
                        statListCBag.AddRange(singleParameterStatistic.DirtyCells.StatList);
                        fixedListCBag.AddRange(singleParameterStatistic.DirtyCells.FixedList);
                    }                   
                }
            });
            dirtyCells.FixedList = fixedListCBag.Distinct().ToList();
            dirtyCells.StatList = statListCBag.Distinct().ToList();
            return dirtyCells.CalculatePercentage(diesCount);
        }

       private SingleStatisticServices.Abstract.SingleStatisticsServiceAbstract SingleStatisticsServiceCreator(Graphic graphic)
       {
            switch (graphic.Type)
            {
                case 1:
                    return new SingleStatisticServices.SingleParameterServiceLNR();
                case 2:
                    return new SingleStatisticServices.SingleParameterServiceHSTG();
            }
            return new SingleStatisticServices.SingleParameterServiceLNR();;

       }

    }
}