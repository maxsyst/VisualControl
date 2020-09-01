using System.Collections.Generic;

namespace VueExample.ViewModels
{
    public class ShortLinkGenerateViewModel
    {
        public System.Guid GeneratedId { get; set; }
        public string WaferId { get; set; }
        public int MeasurementRecordingId { get; set; }
        public string Divider { get; set; }
        public List<GraphicShortLinkViewModel> SelectedGraphics { get; set; } = new List<GraphicShortLinkViewModel>();
        public List<long> SelectedDies { get; set; } = new List<long>();
    }   
}