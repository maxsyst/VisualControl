using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using VueExample.Parsing.Models;

namespace VueExample.ViewModels
{
    public class Graphic4ViewModel
    {
        public string WaferId { get; set; }
        public int MeasurementRecordingId { get; set; }
        public List<Graphic4ParseResult> GraphicData { get; set; }
    }
}