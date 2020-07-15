using System.Collections.Generic;
using VueExample.StatisticsCore;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IGradientService
    {
        GradientViewModel GetGradient(List<SingleParameterStatistic> singleParameterStatisticList, int stepsQuantity, string divider, string statParameterName, List<long> selectedDies);
    }
}