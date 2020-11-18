using System.Collections.Generic;
using Newtonsoft.Json;

namespace VueExample.Models.Charts.Plotly
{
    public class Scatter
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "x")]
        public List<string> XList { get; set; }
        [JsonProperty(PropertyName = "y")]
        public List<double?> YList { get; set; }
        [JsonProperty(PropertyName = "mode")]
        public string Mode { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "showlegend")]
        public bool ShowLegend { get; set; }
        [JsonProperty(PropertyName = "hoverinfo")]
        public string HoverInfo { get; set; }
        [JsonProperty(PropertyName = "line")]
        public Line Line { get; set; }
        [JsonProperty(PropertyName = "marker")]
        public Marker Marker { get; set; }

        public Scatter()
        {
            this.Type = "scatter";
        }
    }
}