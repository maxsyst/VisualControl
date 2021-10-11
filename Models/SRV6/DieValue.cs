using System.Collections.Generic;
using Newtonsoft.Json;
using ZeroFormatter;

namespace VueExample.Models.SRV6
{
    public class DieValue
    {
        [JsonProperty("d")]
        public virtual long? DieId { get; set; }
        [JsonProperty("m")]
        public virtual int? MeasurementRecordingId { get; set; }
        [JsonProperty("g")]
        public virtual int? GraphicId { get; set; }
        [JsonProperty("s")]
        public virtual string State { get; set; }
        [JsonProperty("x")]
        public virtual List<string> XList { get; set; } = new List<string>();
        [JsonProperty("y")]
        public virtual List<string> YList { get; set; } = new List<string>();
    }
}
