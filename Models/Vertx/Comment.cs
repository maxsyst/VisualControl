using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VueExample.Models.Vertx
{
    public class Comment
    {
        [BsonElement("id")]
        public string Id { get; set; }
        [BsonElement("user")]
        public string User { get; set; }
        [BsonElement("text")]
        public string Text { get; set; }

        public Comment()
        {
            Id = Guid.NewGuid().ToString("N");
        }
    }
}