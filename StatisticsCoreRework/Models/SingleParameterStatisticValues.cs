using System.Collections.Generic;

namespace VueExample.StatisticsCoreRework.Models
{
    public class SingleParameterStatisticValues 
    {
        public string StatisticName { get; set; }
        public string Unit { get; set; }
        public Dictionary<long, double> DieStatDictionary { get; set; } = new Dictionary<long, double>();

        public SingleParameterStatisticValues()
        {
            
        }

        public SingleParameterStatisticValues(string name, string unit, List<long?> dieList, List<double> valueList)
        {
            StatisticName = name;
            Unit = unit;
            for (int i = 0; i < dieList.Count; i++)
            {
                DieStatDictionary.Add((long)dieList[i], valueList[i]);
            }
        }
    }
}