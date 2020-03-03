using System.Collections.Generic;

namespace VueExample.ViewModels
{
    public class StandartMeasurementPatternFullViewModel
    {
        public StandartPatternViewModel StandartPattern { get; set; }
        IList<StandartMeasurementPatternViewModel> standartMeasurementPatternList = new List<StandartMeasurementPatternViewModel>();
    }
    
    public class StandartMeasurementPatternViewModel
    {
        public int Id { get; set; }
        public int ElementId { get; set; }
        public int StageId { get; set; }
        public int DividerId { get; set; }
        public int PatternId { get; set; }
        public string Name { get; set; }
        IList<KurbatovParameterViewModel> kpList = new List<KurbatovParameterViewModel>();
    }

    public class KurbatovParameterViewModel
    {
        public KurbatovParameterBordersViewModel KurbatovParameterBorders { get; set; }
        public StandartParameterViewModel StandartParameter { get; set; }
    }

    public class KurbatovParameterBordersViewModel
    {
        public int Id { get; set; }
        public string Lower { get; set; }
        public string Upper { get; set; }
    }

}