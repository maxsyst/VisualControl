using Newtonsoft.Json;

namespace VueExample.ServiceModels
{
    public class WaferMapDie
    {
        [JsonProperty(PropertyName = "x")]
        public double XCoordinate { get; }
        [JsonProperty(PropertyName = "y")]
        public double YCoordinate { get; }
        [JsonProperty(PropertyName = "height")]
        public double Height { get; }
        [JsonProperty(PropertyName = "width")]
        public double Width { get; }
        [JsonProperty(PropertyName = "fill")]
        public string Fill { get; }
        [JsonProperty(PropertyName = "id")]
        public long DieId { get; }
        [JsonProperty(PropertyName = "code")]
        public string Code { get; }
        [JsonProperty(PropertyName = "isActive")]
        public bool IsActive { get; }
       
        public WaferMapDie(double xCoordinate, double yCoordinate, double height, double width, long dieId, string code)
        {
            Code = code;
            DieId = dieId;
            XCoordinate = xCoordinate;
            YCoordinate = yCoordinate;
            Height = height;
            Width = width;
            IsActive = false;
            Fill = "#303030";
        }
    }
}
