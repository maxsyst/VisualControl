using System.Collections.Generic;
using VueExample.Models.Charts.Plotly;

namespace VueExample.Models.Charts
{
    public class ControlChartsData
    {
        public List<Trace> Stock { get; set; }
        public Histogram Histogram { get; set; }
        public List<Scatter> ScatterSPC { get; set; }
        public List<Scatter> ScatterSPCSD { get; set; }
    }
}