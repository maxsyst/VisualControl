using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using VueExample.Parsing.Models;

namespace VueExample.ViewModels
{
    public class Graphic4ViewModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("waferId")]
        public string WaferId { get; set; }
        [BsonElement("measurementRecordingId")]
        public int MeasurementRecordingId { get; set; }
        [BsonElement("graphicData")]
        public List<Graphic4ParseResult> GraphicData { get; set; }
    }
}