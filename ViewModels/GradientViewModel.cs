using System.Collections.Generic;
using VueExample.Extensions;
using VueExample.ViewModels.Abstract;

namespace VueExample.ViewModels
{
    public class GradientViewModel
    {
        public List<GradientStep> GradientSteps { get; set; } = new List<GradientStep>();
    }

    public class ExtremeLowGradientStep : GradientStep
    {
        public ExtremeLowGradientStep(double lowBorder)
        {
            Name = "Low";
            _lowBorder = lowBorder;
            LowBorder = lowBorder.ToGoodFormat();
            Color = "#4527A0";
        }

        private double _lowBorder;
        public string LowBorder { get; private set; }

        public override bool IsInStep(double value)
        {
            return value < _lowBorder;
        }
    }

    public class ExtremeHighGradientStep : GradientStep
    {
        public ExtremeHighGradientStep(double topBorder)
        {
            Name = "High";
            Color = "#F48FB1";
            _topBorder = topBorder;
            TopBorder = topBorder.ToGoodFormat();
        }

        private double _topBorder;
        public string TopBorder { get; private set; }

        public override bool IsInStep(double value)
        {
            return value > _topBorder;
        }
    }


    public class ColorGradientStep : GradientStep
    {
        public ColorGradientStep(int index, double stepSize, double lowBorder, double topBorder, string color)
        {
            Name = $"Step{index + 1}";
            _lowBorder = (lowBorder + (index * stepSize));
            _topBorder = (lowBorder + ((index + 1) * stepSize));
            LowBorder = _lowBorder.ToGoodFormat();
            TopBorder = _topBorder.ToGoodFormat();
            Color = color;

        }
        public double _lowBorder;
        public string LowBorder { get; private set; }
        public double _topBorder;
        public string TopBorder { get; private set; }

        public override bool IsInStep(double value)
        {
            return value >= _lowBorder && value < _topBorder;
        }
    }
}