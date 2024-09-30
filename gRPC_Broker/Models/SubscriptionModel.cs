using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace gRPC_Broker.Models
{
    public class SubscriptionModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    
        [BsonElement("userName")]
        public string UserName { get; set; }

        [BsonElement("topics")]
        public List<string> Topics { get; set; }
    }
}
