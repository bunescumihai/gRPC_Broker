
using Definitions;
using gRPC_Broker.Models;

namespace gRPC_Broker.Repositories;

public interface ISubscriberRepository
{
    Task CreateUser(Credentials credentials);

    Task SubscribeToTopic(string userName, Topic topic);

    Task UnsubscribeFromTopic(string userName, Topic topic);

    Task<bool> SubscriberExists(Credentials credentials);

    Task<List<ArticleModel>> GetArticles(string userName);

    Task CreateToSendInstance(string userName);

    Task<List<Article>> GetArticlesToSend(string userName);

    Task DeleteArticlesToSend(string userName);
}