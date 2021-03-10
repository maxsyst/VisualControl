using System;
using MongoDB.Bson.Serialization.Attributes;

namespace VueExample.Models.Vertx
{
    public class Point
    {
        [BsonElement("generatedId")]
        public string GeneratedId { get; set; }
        [BsonElement("value")]
        public double Value { get; set; }

        [BsonElement("trueDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime TrueDate { get; set; }

        [BsonElement("fromStartDate")]
        public TimeSpan FromStartDate { get; set; }

        public Point()
        {
            Value = double.NaN;
        }

        public Point(double value, DateTime measurementSetCreationDateTime, DateTime trueDate)
        {
            GeneratedId = Guid.NewGuid().ToString("N");
            Value = value;
            TrueDate = trueDate;
            FromStartDate = trueDate - measurementSetCreationDateTime;
        }
    }
}
