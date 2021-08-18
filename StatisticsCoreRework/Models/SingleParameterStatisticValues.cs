using System.Globalization;
using System;
using System.Collections.Generic;

namespace VueExample.StatisticsCoreRework.Models
{
    public class SingleParameterStatisticValues 
    {
        public string StatisticName { get; set; }
        public string Unit { get; set; }
        public Dictionary<long, string> DieStatDictionary { get; set; } = new Dictionary<long, string>();

        public SingleParameterStatisticValues()
        {
            
        }

        public SingleParameterStatisticValues(string name, string unit, List<long?> dieList, List<double> valueList)
        {
            StatisticName = name;
            Unit = unit;
            for (int i = 0; i < dieList.Count; i++)
            {
                if(Double.IsNaN(valueList[i])) 
                {
                     DieStatDictionary.Add((long)dieList[i], "NaN");
            
                }
                else 
                {
                     DieStatDictionary.Add((long)dieList[i], Convert.ToString(valueList[i], CultureInfo.InvariantCulture));
            
                }
           }
        }       
    }
}