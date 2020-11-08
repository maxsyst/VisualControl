namespace VueExample.Models.Charts.Plotly
{
    public class Violin
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "y")]
        public List<double> DataList { get; set; }
        [JsonProperty(PropertyName = "points")]
        public string Points { get; set; }
        [JsonProperty(PropertyName = "box")]
        public Box Box { get; set; }
        [JsonProperty(PropertyName = "boxpoints")]
        public bool Boxpoints { get; set; }
        [JsonProperty(PropertyName = "line")]
        public Line Line { get; set; }
        [JsonProperty(PropertyName = "fillcolor")]
        public string Fillcolor { get; set; }
        [JsonProperty(PropertyName = "opacity")]
        public double Opacity { get; set; }
        [JsonProperty(PropertyName = "meanline")]
        public MeanLine MeanLine { get; set; }
        [JsonProperty(PropertyName = "x0")]
        public string Title { get; set; }

        public Violin()
        {
            this.Type = "violin";
            this.Opacity = 0.6;
            this.Boxpoints = false;
            this.Points = "none";
            this.Line = new Line("black", 2);
            this.MeanLine = new MeanLine();
            this.Box = new Box();
            this.Fillcolor = "#77abff";
        }
    }
}