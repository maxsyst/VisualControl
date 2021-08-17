using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VueExample.Models.SRV6;
using VueExample.StatisticsCore;
using VueExample.StatisticsCoreRework.Models;

namespace VueExample.StatisticsCoreRework.Services
{
    public class SingleParameterServiceHSTG
    {
        private readonly Statistics _statistics;
        public SingleParameterServiceHSTG(Statistics statistics)
        {
            _statistics = statistics;
        }
        public ConcurrentDictionary<string, SingleParameterStatisticValues> CreateSingleParameterStatisticsList(List<DieValue> dieValues, Graphic graphic)
        {
            var dieCommonListDictionary = new ConcurrentDictionary<long?, string>();
            var xList = dieValues.FirstOrDefault().XList;
            var dict = new ConcurrentDictionary<string, SingleParameterStatisticValues>();
            Parallel.ForEach(dieValues, gdv => 
            {
                dieCommonListDictionary.TryAdd(gdv.DieId, gdv.YList.FirstOrDefault());
            });         
            var statisticList = _statistics.GetStatistics(dieCommonListDictionary.Values.ToList(), graphic);
            Parallel.ForEach(statisticList, stat => 
            {
                dict.TryAdd(stat.StatisticsName, new SingleParameterStatisticValues(stat.StatisticsName, stat.Unit, dieCommonListDictionary.Keys.ToList(), stat.FullList));
            });
            return dict;
          
        }
    }
}