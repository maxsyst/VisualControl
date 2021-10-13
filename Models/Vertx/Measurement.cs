using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VueExample.Models.Vertx
{
    public class Measurement
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("Vgate")]
        public string Vgate { get; set; }

        [BsonElement("Vpower")]
        public string Vpower { get; set; }

        [BsonElement("duration")]
        public int DurationSeconds { get; set; }

        [BsonElement("measurementChannel")]
        public string MeasurementChannel { get; set; }

        [BsonElement("temperatureSensor")]
        public string TemperatureSensor { get; set; }

        [BsonElement("matching")]
        public string Matching { get; set; }

        [BsonElement("matchingBoard")]
        public string MatchingBoard { get; set; }

        [BsonElement("goal")]
        public List<string> Goal { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("measurementAttemptId")]
        public ObjectId MeasurementAttemptId { get; set; }

        [BsonElement("creationDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreationDate { get; set; }

        [BsonElement("finishDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime FinishDate { get; set; }

        [BsonElement("lastUpdate")]
        public LastUpdate LastUpdate { get; set; }

        [BsonElement("measurementSetPlusUnits")]
        public IList<MeasurementSetPlusUnit> MeasurementSetPlusUnits { get; set; }

        [BsonElement("notes")]
        public IList<string> Notes { get; set; }

        [BsonElement("comments")]
        public IList<Comment> Comments { get; set; }

        public Measurement()
        {
        }

        public Measurement(string name, string measurementAttemptId)
        {
            MeasurementSetPlusUnits = new List<MeasurementSetPlusUnit>();
            Comments = new List<Comment>();
            Notes = new List<string>();
            MeasurementAttemptId = new ObjectId(measurementAttemptId);
            Name = name;
            Goal = new List<string>();
        }
    }
}
