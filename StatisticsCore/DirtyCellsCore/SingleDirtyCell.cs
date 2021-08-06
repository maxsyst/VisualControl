using System.Runtime.InteropServices;
using System;
using System.Globalization;

namespace VueExample.StatisticsCore.DirtyCellsCore
{
    public class SingleDirtyCell
    {
        public long DieId { get; set; }
        public bool IsDirty { get; set; } = false;
        public Cause Cause { get; set; } = Cause.Unknown;
        public double Difference { get; set; } = 0.0;
        public double TrueValue { get; set; } = 0.0;

        public SingleDirtyCell(long dieId, double trueValue, string lowBorder, string topBorder)
        {
            DieId = dieId;
            TrueValue = trueValue;
            Calculate(lowBorder, topBorder, trueValue);
        }

        private void Calculate(string lowBorder, string topBorder, double trueValue)
        {
            var lowBorderCalculated = lowBorder is null ? -1E24 : double.Parse(lowBorder, CultureInfo.InvariantCulture);
            var topBorderCalculated = topBorder is null ? 1E24 : double.Parse(topBorder, CultureInfo.InvariantCulture);
            if(lowBorderCalculated > trueValue)
            {
                this.Cause = Cause.Lower;
                this.Difference = Math.Abs(lowBorderCalculated - trueValue);
                IsDirty = true;
                return;
            }
            if(topBorderCalculated < trueValue)
            {
                this.Cause = Cause.Bigger;
                this.Difference = Math.Abs(lowBorderCalculated - trueValue);
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