using System.Collections.Generic;
using MathNet.Numerics.Statistics;
using VueExample.StatisticsCore;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6.Interfaces
{
    public interface IGradientService
    {
        GradientViewModel GetGradient(List<SingleParameterStatistic> singleParameterStatisticList, int stepsQuantity, string divider, string statParameterName, List<long> selectedDies);
        Histogram GetHistogram(List<SingleParameterStatistic> singleParameterStatisticList, int stepsQuantity, string statParameterName, List<long> selectedDies);
    }
}