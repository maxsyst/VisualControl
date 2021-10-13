using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VueExample.ViewModels
{
    public class Graphic4UploadingViewModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("waferId")]
        public string WaferId { get; set; }
        [BsonElement("measurementRecordingId")]
        public int MeasurementRecordingId { get; set; }
        [BsonElement("graphicData")]
        public List<Graphic4ParseResultViewModel> GraphicData { get; set; }
    }
}