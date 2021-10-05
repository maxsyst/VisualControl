using Newtonsoft.Json;

namespace VueExample.Models.Charts.Plotly
{
    public class MeanLine
    {
        [JsonProperty(PropertyName = "visible")]
        public bool Visible { get; set; }
        public MeanLine()
        {
            Visible = true;
        }
    }
}