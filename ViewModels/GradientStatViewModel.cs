using System.Collections.Generic;

namespace VueExample.ViewModels
{
    public class GradientStatViewModel
    {
        public int MeasurementRecordingId { get; set; }
        public string Divider { get; set; }
        public string KeyGraphicState { get; set; }
        public string StatParameter { get; set; }
        public double K { get; set; }
        public List<int> SelectedDiesId { get; set; } = new List<int>();
    }
}