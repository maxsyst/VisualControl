using System.Collections.Generic;
using AutoMapper;
using Newtonsoft.Json;

namespace VueExample.ViewModels
{
    public class StandartMeasurementPatternFullViewModel
    {
        public StandartPatternViewModel StandartPattern { get; set; }
        
        [JsonProperty(PropertyName = "standartMeasurementPatternList")]
        public List<StandartMeasurementPatternViewModel> standartMeasurementPatternList { get; set; } = new List<StandartMeasurementPatternViewModel>();
    }
    
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

    public class KurbatovParameterViewModel
    {
        public int Id { get; set; }
        public KurbatovParameterBordersViewModel KurbatovParameterBorders { get; set; }
        public StandartParameterViewModel StandartParameter { get; set; }
    }

    public class KurbatovParameterBordersViewModel
    {
        public int? Id { get; set; }
        public string Lower { get; set; }
        public string Upper { get; set; }
    }

}