using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
namespace VueExample.StatisticsCore
{
    public class DirtyCells
    {
        public List<long?> StatList { get; set; } = new List<long?>();
        public List<long?> FixedList { get; set; } = new List<long?>();
        public string StatPercentageSelected { get; set;}
        public string FixedPercentageSelected { get; set;}
        public string StatPercentageFullWafer { get; set;}
        public string FixedPercentageFullWafer { get; set;}

        public DirtyCells Distinct()
        {
            this.StatList = this.StatList.Distinct().ToList();
            this.FixedList = this.FixedList.Distinct().ToList();
            return this;
        }
        
        public DirtyCells CalculatePercentage(int dieCount)
        {
            this.StatPercentageFullWafer = Convert.ToString(Math.Ceiling((1.0 - (this.StatList.Count / (dieCount + 0.0))) * 100), CultureInfo.InvariantCulture);
            this.FixedPercentageFullWafer = Convert.ToString(Math.Ceiling((1.0 - (this.FixedList.Count / (dieCount + 0.0))) * 100), CultureInfo.InvariantCulture);
            return this;
        }
    }
}