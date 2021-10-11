using VueExample.Extensions;

namespace VueExample.ViewModels.GradientSteps
{
    public class ExtremeHighGradientStep : GradientStep
    {
        private readonly double _topBorder;
        public string TopBorder { get; private set; }
        public ExtremeHighGradientStep(double topBorder)
        {
            Name = "High";
            Color = "#F48FB1";
            _topBorder = topBorder;
            TopBorder = topBorder.ToGoodFormat();
        }
        public override bool IsInStep(double value)
        {
            return value > _topBorder;
        }
    }
}