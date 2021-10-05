using System.Collections.Generic;

namespace VueExample.ViewModels
{
    public class GradientStatViewModel
    {
        public int MeasurementRecordingId { get; set; }
        public string Divider { get; set; }
        public string KeyGraphicState { get; set; }
        public string StatParameter { get; set; }
        public string LowBorder { get; set; }
        public string TopBorder { get; set; }
        public int StepsQuantity { get; set; }
        public void Deconstruct(out int _MeasurementRecordingId, out string _KeyGraphicState, out string _LowBorder, out string _TopBorder) 
        {
            _MeasurementRecordingId = MeasurementRecordingId;
            _KeyGraphicState = KeyGraphicState;
            _LowBorder = LowBorder;
            _TopBorder = TopBorder;     
        }
    }
}