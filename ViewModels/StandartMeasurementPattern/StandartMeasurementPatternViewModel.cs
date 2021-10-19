using System.Collections.Generic;
using AutoMapper;
using Newtonsoft.Json;
using VueExample.ViewModels.Kurbatov;

namespace VueExample.ViewModels.StandartMeasurementPattern
{
    public class StandartMeasurementPatternViewModel
    {
        public int Id { get; set; }
        public int ElementId { get; set; }
        public int StageId { get; set; }
        public int DividerId { get; set; }
        public int PatternId { get; set; }
        public string Name { get; set; }
        public string MslName { get; set; }

        [IgnoreMap]
        [JsonProperty(PropertyName = "kpList")]
        public List<KurbatovParameterViewModel> kpList { get; set; } = new List<KurbatovParameterViewModel>();
    }
}