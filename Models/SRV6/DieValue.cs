using System.Collections.Generic;
using Newtonsoft.Json;

namespace VueExample.Models.SRV6
{
    public class DieValue
    {
        [JsonProperty("d")]
        public long? DieId { get; set; }
        [JsonProperty("m")]
        public int? MeasurementRecordingId { get; set; }
        [JsonProperty("g")]
        public int? GraphicId { get; set; }
        [JsonProperty("s")]
        public string State { get; set; }
        [JsonProperty("x")]
        public List<string> XList { get; set; }
        [JsonProperty("y")]
        public List<string> YList { get; set; }

        public DieValue()
        {
            YList = new List<string>();
            XList = new List<string>();
           
        }

    }
}
