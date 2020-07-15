using System;
using System.Collections.Generic;
using System.Globalization;

namespace VueExample.ViewModels
{
    public class GradientViewModel
    {
        public List<GradientStep> GradientSteps { get; set; } = new List<GradientStep>();
    }


    public abstract class GradientStep
    {
        public string Color { get; protected set;}
        public string BorderDescription { get; protected set; }
        public string Name { get; protected set; }
        public List<long> DieList { get; } = new List<long>();
        public abstract bool IsInStep(double value);
    }

    public class ExtremeLowGradientStep : GradientStep
    {
        public ExtremeLowGradientStep(string lowBorder)
        {
            BorderDescription = $"< {lowBorder}";
            Name = "ExtremeLow";
            LowBorder = Convert.ToDouble(lowBorder, CultureInfo.InvariantCulture);
            Color = "#4527A0";
        }

        public double LowBorder { get; private set;}

        public override bool IsInStep(double value)
        {
            return value < LowBorder;
        }
    }

    public class ExtremeHighGradientStep : GradientStep
    {
        public ExtremeHighGradientStep(string topBorder)
        {
            BorderDescription = $"> {topBorder}";
            Name = "ExtremeHigh";
            Color = "#E91E63";
            TopBorder = Convert.ToDouble(topBorder, CultureInfo.InvariantCulture);
        }

        public double TopBorder { get; private set;}

        public override bool IsInStep(double value)
        {
            return value > TopBorder;
        }
    }


    public class ColorGradientStep : GradientStep
    {
        public ColorGradientStep(int index, double stepSize, string lowBorder, string topBorder, string color)
        {
            Name = $"Step{index + 1}";
            LowBorder = Convert.ToDouble(lowBorder, CultureInfo.InvariantCulture) + index * stepSize;
            TopBorder = Convert.ToDouble(lowBorder, CultureInfo.InvariantCulture) + (index+1) * stepSize;
            Color = color;
            BorderDescription = $"{LowBorder}-{TopBorder}";
        }
        public double LowBorder { get; private set;}
        public double TopBorder { get; private set;}

        public override bool IsInStep(double value)
        {
            return value >= LowBorder && value < TopBorder;
        }
    }
}