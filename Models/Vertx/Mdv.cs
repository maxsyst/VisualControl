using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VueExample.Models.Vertx
{
    public class Mdv
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("waferId")]
        public string WaferId { get; set; }
        [BsonElement("code")]
        public string Code { get; set; }
        [BsonElement("description")]
        public string Description { get; set; }
    }
}