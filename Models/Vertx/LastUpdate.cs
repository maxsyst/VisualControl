using System;
using MongoDB.Bson.Serialization.Attributes;

namespace VueExample.Models.Vertx
{
    public class LastUpdate
    {
        [BsonElement("value")] public double Value { get; set; }

        [BsonElement("date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }

        public LastUpdate()
        {

        }

        public LastUpdate(double value)
        {
            Value = value;
            Date = DateTime.Now;
        }

        public LastUpdate(double value, DateTime creationDate)
        {
            Value = value;
            Date = creationDate;
        }
    }
}