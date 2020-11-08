namespace VueExample.Models.Charts.Plotly
{
    public class Marker
    {
        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; }
        [JsonProperty(PropertyName = "size")]
        public int Size { get; set; }
        [JsonProperty(PropertyName = "symbol")]
        public string Symbol { get; set; }
        [JsonProperty(PropertyName = "opacity")]
        public double Opacity { get; set; }
        [JsonProperty(PropertyName = "line")]
        public Line Line { get; set; }
        
        public Marker()
        {
            this.Color = "rgb(107,174,214)";
        }

        public Marker(double opacity)
        {
            this.Color = "rgb(107,174,214)";
            this.Opacity = opacity;
        }
    }
}