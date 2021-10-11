using VueExample.Extensions;

namespace VueExample.ViewModels.GradientSteps
{
    public class ExtremeLowGradientStep : GradientStep
    {
        private readonly double _lowBorder;
        public string LowBorder { get; private set; }
        public ExtremeLowGradientStep(double lowBorder)
        {
            Name = "Low";
            _lowBorder = lowBorder;
            LowBorder = lowBorder.ToGoodFormat();
            Color = "#4527A0";
        }
        public override bool IsInStep(double value)
        {
            return value < _lowBorder;
        }
    }
}