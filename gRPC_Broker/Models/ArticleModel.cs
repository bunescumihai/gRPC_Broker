using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Definitions;

namespace gRPC_Broker.Models
{
    public class ArticleModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("userName")] 
        public string UserName { get; set; }

        [BsonElement("topic")]
        public TopicModel TopicModel { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        public static Article GetArticleMessageFromArticleModel(ArticleModel articleModel)
        {
            return new Article
            {
                Topic = TopicModel.GetTopicMessageFromTopicModel(articleModel.TopicModel),
                Content = articleModel.Content
            };
        }

        public static ArticleModel GetArticleModelFromArticleMessage(string UserName, Article article)
        {
            return new ArticleModel {
                UserName = UserName,
                TopicModel = TopicModel.GetTopicModelFromTopicMessage(article.Topic),
                Content = article.Content
            };
        }
    }
}
