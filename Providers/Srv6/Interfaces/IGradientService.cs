using VueExample.StatisticsCoreRework.Models;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IGradientService
    {
        GradientViewModel GetGradient(SingleParameterStatisticValues singleParameterStatisticValues, int stepsQuantity, string lowborder, string topBorder, string divider);
    }
}