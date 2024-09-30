using Definitions;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace gRPC_Broker.Models
{
    public class TopicModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("subscribedUsers")]
        public List<string> SubscribedUsers { get; set; }

        public static Topic GetTopicMessageFromTopicModel( TopicModel topic)
        {
            return new Topic { Name = topic.Name };
        }

        public static TopicModel GetTopicModelFromTopicMessage( Topic topic)
        {
            return new TopicModel { Name = topic.Name };
        }
    }
}
