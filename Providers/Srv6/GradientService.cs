using System;
using System.Globalization;
using System.Linq;
using VueExample.Color;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.StatisticsCoreRework;
using VueExample.StatisticsCoreRework.Models;
using VueExample.ViewModels;
using VueExample.ViewModels.GradientSteps;

namespace VueExample.Providers.Srv6
{
    public class GradientService : IGradientService
    {
        private readonly IColorService _colorService;
        public GradientService(IColorService colorService)
        {
            _colorService = colorService;
        }

        public GradientViewModel GetGradient(SingleParameterStatisticValues singleParameterStatisticValues, int stepsQuantity, string lowBorder, string topBorder, string divider)
        {
            var gradientViewModel = new GradientViewModel();
            var colorList = stepsQuantity <= 32 ? _colorService.GetGradientColors().Select(x => x.Hex).ToList() : Enumerable.Repeat("#3F51B5", stepsQuantity).ToList();
            if(singleParameterStatisticValues is null)
            {
                return new GradientViewModel();
            }
            var lowBorderDouble = Convert.ToDouble(lowBorder, CultureInfo.InvariantCulture);
            var topBorderDouble = Convert.ToDouble(topBorder, CultureInfo.InvariantCulture);
            var stepSize = Math.Abs(topBorderDouble - lowBorderDouble) / stepsQuantity;
            var extremeLowGradientStep = new ExtremeLowGradientStep(lowBorderDouble);
            gradientViewModel.GradientSteps.Add(extremeLowGradientStep);
            for (int i = 0; i < stepsQuantity; i++)
            {
                gradientViewModel.GradientSteps.Add(new ColorGradientStep(i, stepSize, lowBorderDouble, topBorderDouble, colorList[i]));
            }
            var extremeHighGradientStep = new ExtremeHighGradientStep(topBorderDouble);
            gradientViewModel.GradientSteps.Add(extremeHighGradientStep);

            foreach (var dieStat in singleParameterStatisticValues.DieStatDictionary)
            {
                var step = extremeLowGradientStep.LowBorder == extremeHighGradientStep.TopBorder
                           ? gradientViewModel.GradientSteps[(int)stepsQuantity / 2]
                           : gradientViewModel.GradientSteps
                                              .FirstOrDefault(x => x.IsInStep(Divider(Convert.ToDouble(dieStat.Value, CultureInfo.InvariantCulture),
                                                              singleParameterStatisticValues.DividerProfile, divider)));
                step.DieList.Add(dieStat.Key);
            }
            return gradientViewModel;
        }

        private double Divider(double value, DividerProfile profile, string divider)
        {
            if(profile == DividerProfile.WithDivider)
            {
               return value / Convert.ToDouble(divider, CultureInfo.InvariantCulture);
            }
            if(profile == DividerProfile.ROnFamily)
            {
                return value / (1/Convert.ToDouble(divider, CultureInfo.InvariantCulture));
            }
            return value;
        }
    }
}