using System.Collections.Generic;

namespace VueExample.ViewModels
{
    public class GradientViewModel
    {
        public List<DieGradientStep> DieSteps { get; set; } = new List<DieGradientStep>();
        public List<GradientStep> GradientSteps { get; set; } = new List<GradientStep>();
    }

    public class DieGradientStep
    {
        public long DieId { get; set; }
        public int Step { get; set; }
    }

    public class GradientStep
    {
        public int Step { get; set; }
        public double LowBorder { get; set; }
        public double TopBorder { get; set; }
    }
}