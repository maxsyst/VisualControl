using System.Collections.Generic;
using VueExample.ServiceModels;

namespace VueExample.ViewModels
{
    public class FormedMapViewModel
    {
        public List<WaferMapDie> WaferMapDies { get; set; } = new List<WaferMapDie>();
        public string Orientation { get; set; }
    }
}