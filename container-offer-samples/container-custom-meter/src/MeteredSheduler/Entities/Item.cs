using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace MeteredSheduler.Entities
{
    public class Item
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("DimensionId")]
        public string DimensionId { get; set; }
        [BsonElement("CreatedDate")]
        public DateTime CreatedDate { get; set; }
        [BsonElement("Count")]
        public int Count { get; set; }
        [BsonElement("MeterProcessStatus")]
        public bool MeterProcessStatus { get; set; }
    }
}
