using System;
using VueExample.StatisticsCoreRework.Models;

namespace VueExample.StatisticsCoreRework.Abstract
{
    public interface IStatisticCalculationService
    {
        SingleParameterStatisticCalculated Calculate(SingleParameterStatisticValues singleParameterStatisticValues, double divider);
    }
}