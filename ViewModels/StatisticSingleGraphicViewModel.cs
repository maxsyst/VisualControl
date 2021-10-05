using System.Collections.Generic;
namespace VueExample.ViewModels
{
    public class StatisticSingleGraphicViewModel
    {
        public int MeasurementId  { get; set; }
        public string KeyGraphicState { get; set; }
        public List<long?> dieIdList {get; set; }
        public string Divider { get; set; }
        public double K { get; set; }

        public StatisticSingleGraphicViewModel()
        {
            this.dieIdList = new List<long?>();
        }

        public void Deconstruct(out int _MeasurementId, out string _KeyGraphicState, out double _K) 
        {
            _MeasurementId = MeasurementId;
            _KeyGraphicState = KeyGraphicState;
            _K = K;
        }
    }
}