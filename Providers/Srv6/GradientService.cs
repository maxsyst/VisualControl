using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MathNet.Numerics.Statistics;
using VueExample.Color;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.StatisticsCore;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6
{
    public class GradientService : IGradientService
    {
        private readonly IColorService _colorService;
        public GradientService(IColorService colorService)
        {
            _colorService = colorService;
        }
        public GradientViewModel GetGradient(List<SingleParameterStatistic> singleParameterStatisticList, int stepsQuantity, string divider, string statParameterName, List<long> selectedDies)
        {
            var singleStatistic = singleParameterStatisticList.FirstOrDefault(x => x.Name == statParameterName);
            var dataDescriptiveStatistics = new DataDescriptiveStatistics(singleStatistic.valueList);
            var gradientViewModel = new GradientViewModel();
            var colorList = stepsQuantity <= 32 ? _colorService.GetGradientColors().Select(x => x.Hex).ToList() : Enumerable.Repeat("#3F51B5", stepsQuantity).ToList();
            if(singleStatistic == null) 
            {
                return new GradientViewModel();
            }
            var stepSize = Math.Abs(Convert.ToDouble(singleStatistic.TopBorderStat, CultureInfo.InvariantCulture) -
                                    Convert.ToDouble(singleStatistic.LowBorderStat, CultureInfo.InvariantCulture)) / stepsQuantity;
            var singleDieValues = new List<SingleDieValue>();
            for (int i = 0; i < singleStatistic.dieList.Count; i++)
            {
                if(selectedDies.Contains((long)singleStatistic.dieList[i])) 
                {
                    singleDieValues.Add(new SingleDieValue{DieId = (long)singleStatistic.dieList[i], Value = singleStatistic.valueList[i]});
                }               
            }

            gradientViewModel.GradientSteps.Add(new ExtremeLowGradientStep(singleStatistic.LowBorderStat));

            for (int i = 0; i < stepsQuantity; i++)
            {
                gradientViewModel.GradientSteps.Add(new ColorGradientStep(i, stepSize, singleStatistic.LowBorderStat, singleStatistic.TopBorderStat, colorList[i]));
            }
            
            gradientViewModel.GradientSteps.Add(new ExtremeHighGradientStep(singleStatistic.TopBorderStat));

            foreach (var singleDieValue in singleDieValues)
            {
                var step = gradientViewModel.GradientSteps.FirstOrDefault(x => x.IsInStep(singleDieValue.Value));
                step.DieList.Add(singleDieValue.DieId);
            }
            return gradientViewModel;
        }

        public Histogram GetHistogram(List<SingleParameterStatistic> singleParameterStatisticList, int stepsQuantity, string statParameterName, List<long> selectedDies)
        {
            var singleStatistic = singleParameterStatisticList.FirstOrDefault(x => x.Name == statParameterName);
            var dataDescriptiveStatistics = new DataDescriptiveStatistics(singleStatistic.valueList);
            return dataDescriptiveStatistics.GetHistogramFromList(singleStatistic.valueList, stepsQuantity);
        }
    }

    public class SingleDieValue 
    {
        public long DieId { get; set; }
        public double Value { get; set; }
        
    }
}