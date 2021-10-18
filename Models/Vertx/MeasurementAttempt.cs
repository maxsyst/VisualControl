using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VueExample.Models.Vertx
{
    public class MeasurementAttempt
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("mdvId")]
        public ObjectId MdvId { get; set; }

        [BsonElement("rootMeasurementId")]
        public string RootMeasurementId { get; set; }

        [BsonElement("measurements")]
        public IList<string> MeasurementsId { get; set; }

        [BsonElement("measurementResult")]
        public MeasurementResult MeasurementResult { get; set; }

        public MeasurementAttempt()
        {
            MeasurementsId = new List<string>();
            MeasurementResult = new MeasurementResult();
        }

        public MeasurementAttempt(string mdvId, string name) : this()
        {
            MdvId = new ObjectId(mdvId);
            Name = name;
        }
    }
}
