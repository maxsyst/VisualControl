using System.Collections.Generic;
using Newtonsoft.Json;

namespace VueExample.ViewModels.StandartMeasurementPattern
{
    public class StandartMeasurementPatternFullViewModel
    {
        public StandartPatternViewModel StandartPattern { get; set; }

        [JsonProperty(PropertyName = "standartMeasurementPatternList")]
        public List<StandartMeasurementPatternViewModel> standartMeasurementPatternList { get; set; } = new List<StandartMeasurementPatternViewModel>();
    }
}