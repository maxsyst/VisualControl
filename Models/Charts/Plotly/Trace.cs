using System.Collections.Generic;
using Newtonsoft.Json;

namespace VueExample.Models.Charts.Plotly
{
    public class Trace
    {
        [JsonProperty(PropertyName = "y")]
        public List<double> ValueList { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "marker")]
        public Marker Marker { get; set; }
        [JsonProperty(PropertyName = "boxpoints")]
        public string Boxpoint { get; set; }
        [JsonProperty(PropertyName = "boxmean")]
        public string Boxmean { get; set; }

        public Trace(List<double> valueList, string name, string boxmean, string boxpoints)
        {
            ValueList = valueList;
            Type = "box";
            Name =  name;
            Marker = new Marker{Size = 5, Symbol = "circle", Opacity = 0.5};
            Boxpoint = boxpoints;
            Boxmean = boxmean;
        }
    }
}