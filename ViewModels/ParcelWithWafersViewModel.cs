using System.Collections.Generic;
using Newtonsoft.Json;

namespace VueExample.ViewModels
{
    public class ParcelWithWafersViewModel
    {
        [JsonProperty("id")]
        public int ParcelId { get; set; }
        [JsonProperty("name")]
        public string ParcelName { get; set; }

        [JsonProperty("children")]
        public List<SimpleWaferId> ChildrenWafers { get; set; }
    }

    public class SimpleWaferId
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}