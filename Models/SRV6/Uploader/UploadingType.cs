using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using VueExample.Models.SRV6.Uploader.Models;

namespace VueExample.Models.SRV6.Uploader
{
    public class UploadingType
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("type")]
        public string Type { get; set; }
        [BsonElement("graphics")]
        public IList<Graphic4Type> Graphics { get; set; }
    }
}