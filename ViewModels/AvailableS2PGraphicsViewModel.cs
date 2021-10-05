using System.Collections.Generic;
using VueExample.Models.SRV6;

namespace VueExample.ViewModels
{
    public class AvailableS2PGraphicsViewModel
    {
        public List<Graphic> AvailableGraphics { get; set; } = new List<Graphic>();
        public Dictionary<string, Graphic> CurrentGraphics { get; set; } = new Dictionary<string, Graphic>();
    }
}