using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace gRPC_Broker.Models.Projections
{
    public class TopicProjection
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }
    }
}