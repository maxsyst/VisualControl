using Newtonsoft.Json;

namespace VueExample.ServiceModels
{
    public class DefectiveDie
    {
        [JsonProperty(PropertyName = "dieId")]
        public long DieId { get; set; }
        [JsonProperty(PropertyName = "color")]
        public string HexColor { get; set; }
    }
}
