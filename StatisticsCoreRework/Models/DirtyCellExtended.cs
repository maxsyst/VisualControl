using System;
using System.Globalization;

namespace VueExample.StatisticsCoreRework.Models
{
    public class DirtyCellExtended
    {
        public long DieId { get; set; }
        public bool IsDirty { get; set; } = false;
        public Cause Cause { get; set; } = Cause.Unknown;
        public string Difference { get; set; } = "0.0";
        public string TrueValue { get; set; } = "0.0";

        public DirtyCellExtended()
        {
        }

        public DirtyCellExtended(long dieId, double trueValue, string lowBorder, string topBorder)
        {
            DieId = dieId;
            TrueValue = Convert.ToString(trueValue, CultureInfo.InvariantCulture);
            Calculate(lowBorder, topBorder, trueValue);
        }

        private void Calculate(string lowBorder, string topBorder, double trueValue)
        {
            var lowBorderCalculated = String.IsNullOrEmpty(lowBorder) ? -1E24 : double.Parse(lowBorder, CultureInfo.InvariantCulture);
            var topBorderCalculated = String.IsNullOrEmpty(topBorder) ? 1E24 : double.Parse(topBorder, CultureInfo.InvariantCulture);
            if(lowBorderCalculated > trueValue)
            {
                this.Cause = Cause.Lower;
                this.Difference = Convert.ToString(Math.Abs(lowBorderCalculated - trueValue), CultureInfo.InvariantCulture);
                IsDirty = true;
                return;
            }
            if(topBorderCalculated < trueValue)
            {
                this.Cause = Cause.Bigger;
                this.Difference =  Convert.ToString(Math.Abs(topBorderCalculated - trueValue), CultureInfo.InvariantCulture);
                IsDirty = true;
                return;
            }
            if(double.IsNaN(trueValue)) {
                this.Cause = Cause.isNaN;
                IsDirty = true;
                return;
            }
        }
    }

    public enum Cause
    {
        Lower,
        Bigger,
        isNaN,
        Unknown
    }
}