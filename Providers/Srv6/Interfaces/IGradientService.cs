using System.Collections.Generic;
using VueExample.StatisticsCoreRework.Models;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IGradientService
    {
        GradientViewModel GetGradient(SingleParameterStatisticValues singleParameterStatisticValues, int stepsQuantity, double k, string divider, List<long> selectedDies);
    }
}