using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VueExample.StatisticsCoreRework.Models
{
    public class SingleParameterStatisticValues 
    {
        public string StatisticName { get; set; }
        public string Unit { get; set; }
        public DividerProfile DividerProfile { get; set; } = DividerProfile.WithoutDivider;
        public Dictionary<long, string> DieStatDictionary { get; set; } = new Dictionary<long, string>();

        public SingleParameterStatisticValues()
        {
            
        }

        public SingleParameterStatisticValues(string name, string unit, DividerProfile dividerProfile, List<long?> dieList, List<double> valueList)
        {
            StatisticName = name;
            Unit = unit;
            DividerProfile = dividerProfile;
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

        public void Deconstruct(out string _StatisticName, out string _Unit, out DividerProfile _DividerProfile, out Dictionary<long, string> _DieStatDictionary, out List<double> _Values) 
        {
            _StatisticName = StatisticName;
            _Unit = Unit;
            _DividerProfile = DividerProfile;
            _DieStatDictionary = DieStatDictionary;
            _Values = DieStatDictionary.Values.Select(x => Convert.ToDouble(x, CultureInfo.InvariantCulture)).ToList();

        }
    }
}