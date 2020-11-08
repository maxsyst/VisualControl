namespace VueExample.Models.Charts.Plotly
{
    public class Line
    {
        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; }
        [JsonProperty(PropertyName = "width")]
        public int Width { get; set; }
        [JsonProperty(PropertyName = "dash")]
        public string Dash { get; set; }
        
        public Line()
        {
            this.Color = "rgb(107,174,214)";
        }

        public Line(string color)
        {
            this.Color = color;
        }

        public Line(string color, int width)
        {
            this.Color = color;
            this.Width = width;
        }
    }
}