using VueExample.Extensions;

namespace VueExample.ViewModels.GradientSteps
{
    public class ColorGradientStep : GradientStep
    {
        public readonly double _lowBorder;
        public string LowBorder { get; private set; }
        public readonly double _topBorder;
        public string TopBorder { get; private set; }
        public ColorGradientStep(int index, double stepSize, double lowBorder, double topBorder, string color)
        {
            Name = $"Step{index + 1}";
            _lowBorder = (lowBorder + (index * stepSize));
            _topBorder = (lowBorder + ((index + 1) * stepSize));
            LowBorder = _lowBorder.ToGoodFormat();
            TopBorder = _topBorder.ToGoodFormat();
            Color = color;
        }
        public override bool IsInStep(double value)
        {
            return value >= _lowBorder && value < _topBorder;
        }
    }
}