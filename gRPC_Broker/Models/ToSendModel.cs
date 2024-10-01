using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace gRPC_Broker.Models
{
    public class ToSendModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("userName")]
        public string UserName { get; set; }

        [BsonElement("articles")]

        public List<string> Articles { get; set; } = new List<string>();
    }
}
