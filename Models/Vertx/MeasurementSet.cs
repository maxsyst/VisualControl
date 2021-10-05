using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VueExample.Models.Vertx
{
    public class MeasurementSet
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("measurementSetPlusUnitId")]
        public string MeasurementSetPlusUnitId { get; set; }


        [BsonElement("points")]
        public List<Point> Points { get; set; }

        public MeasurementSet(string measurementSetPlusUnitId)
        {
            MeasurementSetPlusUnitId = measurementSetPlusUnitId;
            Points = new List<Point>();
        }

    }
}
