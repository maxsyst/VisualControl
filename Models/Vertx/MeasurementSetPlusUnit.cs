using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace VueExample.Models.Vertx
{
    public class MeasurementSetPlusUnit
    {
        [BsonElement("generatedId")]
        public string GeneratedId { get; set; }

        [BsonElement("characteristic")]
        public Characteristic Characteristic { get; set; }

        [BsonElement("measurementSetsIds")]
        public IList<string> MeasurementSetIds { get; set; }

        [BsonElement("lastUpdate")]
        public LastUpdate LastUpdate { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("creationDate")]
        public DateTime CreationDate { get; set; }

        public MeasurementSetPlusUnit(Characteristic characteristic, int quantTimeSeconds, DateTime creationDate)
        {
            Characteristic = characteristic;
            GeneratedId = Guid.NewGuid().ToString("N");
            LastUpdate = new LastUpdate();
            MeasurementSetIds = new List<string>();
            CreationDate = creationDate;
        }
    }

}
