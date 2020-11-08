namespace VueExample.Models.Charts.Plotly
{
    public class Histogram
    {
        [JsonProperty(PropertyName = "x")]
        public List<double> Data { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "histnorm")]
        public string Histnorm { get; set; }

        [JsonProperty(PropertyName = "nbinsx")]
        public Int32 Nbinsx { get; set; }

        [JsonProperty(PropertyName = "autobinx")]
        public bool Autobinx { get; set; }

        [JsonProperty(PropertyName = "marker")]
        public Marker Marker { get; set; }

        public Histogram(List<double> dataList )
        {
            this.Data = dataList;
            this.Histnorm = "probability";
            this.Marker = new Marker(0.5);
            this.Type = "histogram";
            this.Autobinx = true;
            this.Nbinsx = 30;
        }
    }
}