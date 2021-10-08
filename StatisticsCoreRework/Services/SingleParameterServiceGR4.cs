using System.Globalization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VueExample.Models.SRV6;
using VueExample.StatisticsCoreRework.Models;

namespace VueExample.StatisticsCoreRework.Services
{
    public class SingleParameterServiceGR4
    {
        private readonly Statistics _statistics;
        public SingleParameterServiceGR4(Statistics statistics)
        {
            _statistics = statistics;
        }

        public Dictionary<string, SingleParameterStatisticValues> CreateSingleParameterStatisticsList(List<DieValue> dieValues, Graphic graphic, string keyGraphicState)
        {
            var state = keyGraphicState.Split('_')[1];
            var dieCommonListDictionary = new ConcurrentDictionary<long?, List<string>>();
            var xList = dieValues.FirstOrDefault().XList;
            var dict = new ConcurrentDictionary<string, SingleParameterStatisticValues>();
            Parallel.ForEach(dieValues, gdv => 
            {
                dieCommonListDictionary.TryAdd(gdv.DieId, gdv.YList);
            });         
            var statisticList = _statistics.GetStatistics(dieValues.FirstOrDefault().XList.Select(x => Convert.ToDouble(x, CultureInfo.InvariantCulture)).ToList(), dieCommonListDictionary.Values.Select(x => x.Select(l => Convert.ToDouble(l, CultureInfo.InvariantCulture)).ToList()).ToList(), graphic, 1.0, state);
            Parallel.ForEach(statisticList, stat => 
            {
                dict.TryAdd(stat.StatisticsName, new SingleParameterStatisticValues(stat.StatisticsName, stat.Unit, stat.NeedDivider, dieCommonListDictionary.Keys.ToList(), stat.FullList));
            });
            return dict.ToDictionary(x => x.Key, v => v.Value);
        }
    }
}