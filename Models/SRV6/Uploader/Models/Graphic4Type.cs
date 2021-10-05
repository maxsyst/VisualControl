using MongoDB.Bson.Serialization.Attributes;

namespace VueExample.Models.SRV6.Uploader.Models
{
    public class Graphic4Type
    {
        [BsonElement("graphicSRV6Id")]
        public int GraphicSRV6Id { get; set; }
        [BsonElement("graphicMode")]
        public string GraphicMode { get; set; }
    }
}