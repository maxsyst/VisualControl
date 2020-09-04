using System.Linq;
using System.Collections.Generic;
using System;
using VueExample.Models.SRV6;
using System.Globalization;

namespace VueExample.StatisticsCore
{
    public class SingleParameterStatistic
    {
        public string Name { get; set; }
        public List<long?> dieList {get; }
        public List<double> valueList {get; }
        public string LowBorderStat { get; set; }
        public string TopBorderStat { get; set; }
        public string LowBorderFixed { get; set; }
        public string TopBorderFixed{ get; set; }
        public string AverageFixed { get; set; }
        public bool IsHasFixed { get; set; } = false;
        public DirtyCells DirtyCells{get; set; } 

        public SingleParameterStatistic(string name, List<long?> dieList, List<double> valueList, double k)
        {
            this.dieList = new List<long?>(dieList);
            this.Name = name;
            this.valueList = new List<double>(valueList);
            this.DirtyCells = new DirtyCells();
            CalculateDirtyCellsStat(k);
        }

        public SingleParameterStatistic(string name, List<long?> dieList, List<double> valueList, DirtyCells originDirtyCells)
        {
            this.dieList = new List<long?>(dieList);
            this.Name = name;
            this.valueList = new List<double>(valueList);
            this.DirtyCells = new DirtyCells();
            this.DirtyCells.StatList = originDirtyCells.StatList.Intersect(dieList).ToList();
            this.DirtyCells.FixedList = originDirtyCells.FixedList.Intersect(dieList).ToList();
        }
    

        private void CalculateDirtyCellsStat(double k)
        {
            var dds = new DataDescriptiveStatistics(valueList.Where(v => !Double.IsNaN(v)).ToList());
            for (int i = 0; i < valueList.Count; i++)
            {
                if((dds.Quartile3Double + k*dds.IQRDouble < valueList[i] || dds.Quartile1Double - dds.IQRDouble*k > valueList[i] || double.IsNaN(valueList[i])))
                {
                    this.DirtyCells.StatList.Add(dieList[i]);
                }
            }
            this.LowBorderStat = GetFormat(dds.Quartile1Double - dds.IQRDouble * k);
            this.TopBorderStat = GetFormat(dds.Quartile3Double + k * dds.IQRDouble);
        }

        public SingleParameterStatistic CalculateDirtyCellsFixed(StatParameterForStage statParameterForStage)
        {
            if(statParameterForStage != null)
            {
                this.LowBorderFixed = !String.IsNullOrEmpty(statParameterForStage.MinAverage) ? statParameterForStage.MinAverage : "Не установлено";
                this.TopBorderFixed = !String.IsNullOrEmpty(statParameterForStage.MaxAverage) ? statParameterForStage.MaxAverage : "Не установлено";
                CalculateFixedList();
            }

            return this;
            
        }

        private void CalculateFixedList()
        {
            var lowBorder = this.LowBorderFixed == "Не установлено" ? 1E-24 : double.Parse(this.LowBorderFixed, CultureInfo.InvariantCulture);
            var topBorder = this.TopBorderFixed == "Не установлено" ? 1E24 : double.Parse(this.TopBorderFixed, CultureInfo.InvariantCulture);
            for (int i = 0; i < valueList.Count; i++)
            {
                if(lowBorder > valueList[i] || topBorder < valueList[i] || double.IsNaN(valueList[i]))
                {
                     this.DirtyCells.FixedList.Add(dieList[i]);
                }
            }
            IsHasFixed = true;
        }

        private string GetFormat(double number)
        {
            if (Math.Abs(number) < 1E-22 || Math.Abs(number) > 1E22)
            {
                return String.Empty;
            }
            if ((Math.Abs(number) >= 10000 || Math.Abs(number) < 1E-2) && Math.Abs(number - 0) > 1E-20)
            {
                return number.ToString("0.00E0");
            }
            return number.ToString("0.000");
        }
    }
}