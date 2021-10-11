using System.Collections.Generic;

namespace VueExample.ViewModels.GradientSteps
{
    public abstract class GradientStep
    {
        public string Color { get; protected set; }
        public string Name { get; protected set; }
        public List<long> DieList { get; } = new List<long>();
        public abstract bool IsInStep(double value);
    }
}