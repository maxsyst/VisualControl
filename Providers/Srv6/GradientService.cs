using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VueExample.Color;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.StatisticsCore;
using VueExample.StatisticsCoreRework.Models;
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

        public GradientViewModel GetGradient(SingleParameterStatisticValues singleParameterStatisticValues, int stepsQuantity, double k, string divider, List<long> selectedDies)
        {
            var gradientViewModel = new GradientViewModel();
            var colorList = stepsQuantity <= 32 ? _colorService.GetGradientColors().Select(x => x.Hex).ToList() : Enumerable.Repeat("#3F51B5", stepsQuantity).ToList();
            if(singleParameterStatisticValues is null) 
            {
                return new GradientViewModel();
            }
            var dataDescriptiveStatistics = new DataDescriptiveStatistics(singleParameterStatisticValues.GetValues());
            var lowBorder = dataDescriptiveStatistics.Quartile1Double - k * dataDescriptiveStatistics.IQRDouble;
            var topBorder = dataDescriptiveStatistics.Quartile3Double + k * dataDescriptiveStatistics.IQRDouble;
            var stepSize = Math.Abs(topBorder - lowBorder) / stepsQuantity;
            gradientViewModel.GradientSteps.Add(new ExtremeLowGradientStep(lowBorder));
            for (int i = 0; i < stepsQuantity; i++)
            {
                gradientViewModel.GradientSteps.Add(new ColorGradientStep(i, stepSize, lowBorder, topBorder, colorList[i]));
            }
            
            gradientViewModel.GradientSteps.Add(new ExtremeHighGradientStep(topBorder));

            foreach (var dieStat in singleParameterStatisticValues.DieStatDictionary)
            {
                var step = gradientViewModel.GradientSteps.FirstOrDefault(x => x.IsInStep(Convert.ToDouble(dieStat.Value, CultureInfo.InvariantCulture)));
                step.DieList.Add(dieStat.Key);
            }
            return gradientViewModel;
        }
    }

}