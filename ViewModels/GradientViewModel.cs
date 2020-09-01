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

        protected string GetFormat(double number)
        {
            if (Math.Abs(number) < 1E-22 || Math.Abs(number) > 1E22)
            {
                return String.Empty;
            }
            if ((Math.Abs(number) >= 10000 || Math.Abs(number) < 1E-2) && Math.Abs(number - 0) > 1E-20)
            {
                return number.ToString("0.000E0");
            }
            return number.ToString("0.0000");
        }
    }

    public class ExtremeLowGradientStep : GradientStep
    {
        public ExtremeLowGradientStep(string lowBorder)
        {
            Name = "Low";
            LowBorder = Convert.ToDouble(lowBorder.Replace(',', '.'), CultureInfo.InvariantCulture);
            Color = "#4527A0";
            BorderDescription = $"< {GetFormat(LowBorder)}";
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
            Name = "High";
            Color = "#E91E63";
            TopBorder = Convert.ToDouble(topBorder.Replace(',', '.'), CultureInfo.InvariantCulture);
            BorderDescription = $"> {GetFormat(TopBorder)}";
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
            LowBorder = Convert.ToDouble(lowBorder.Replace(',', '.'), CultureInfo.InvariantCulture) + index * stepSize;
            TopBorder = Convert.ToDouble(lowBorder.Replace(',', '.'), CultureInfo.InvariantCulture) + (index+1) * stepSize;
            Color = color;
            BorderDescription = $"{GetFormat(LowBorder)}->{GetFormat(TopBorder)}";
        }
        public double LowBorder { get; private set;}
        public double TopBorder { get; private set;}

        public override bool IsInStep(double value)
        {
            return value >= LowBorder && value < TopBorder;
        }
    }
}