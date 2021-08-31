using System.Collections.Generic;
using VueExample.Extensions;

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
        public ExtremeLowGradientStep(double lowBorder)
        {
            Name = "Low";
            LowBorder = lowBorder;
            Color = "#4527A0";
            BorderDescription = $"< {LowBorder.ToGoodFormat()}";
        }

        public double LowBorder { get; private set;}

        public override bool IsInStep(double value)
        {
            return value < LowBorder;
        }
    }

    public class ExtremeHighGradientStep : GradientStep
    {
        public ExtremeHighGradientStep(double topBorder)
        {
            Name = "High";
            Color = "#E91E63";
            TopBorder = topBorder;
            BorderDescription = $"> {(TopBorder.ToGoodFormat())}";
        }

        public double TopBorder { get; private set;}

        public override bool IsInStep(double value)
        {
            return value > TopBorder;
        }
    }


    public class ColorGradientStep : GradientStep
    {
        public ColorGradientStep(int index, double stepSize, double lowBorder, double topBorder, string color)
        {
            Name = $"Step{index + 1}";
            LowBorder = lowBorder + index * stepSize;
            TopBorder = lowBorder + (index+1) * stepSize;
            Color = color;
            BorderDescription = $"{LowBorder.ToGoodFormat()}->{TopBorder.ToGoodFormat()}";
        }
        public double LowBorder { get; private set;}
        public double TopBorder { get; private set;}

        public override bool IsInStep(double value)
        {
            return value >= LowBorder && value < TopBorder;
        }
    }
}