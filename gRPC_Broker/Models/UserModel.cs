using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using gRPC_Broker.Definitions;

namespace gRPC_Broker.Models
{
    public class UserModel
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("userName")]
        public string UserName { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("role")]
        public UserRole Role { get; set; }

    }
}
