using System;
using System.Collections.Generic;
using VueExample.ViewModels;

namespace VueExample.Models.SRV6
{
    public class ShortLink
    {
        public Guid GeneratedId { get; set; }
        public string WaferId { get; set; }
        public int MeasurementRecordingId { get; set; }
        public Divider Divider { get; set; }
        public List<GraphicShortLinkViewModel> SelectedGraphics { get; set; } = new List<GraphicShortLinkViewModel>();
        public List<long> SelectedDies { get; set; } = new List<long>();
    }
}