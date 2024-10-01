
using Definitions;
using gRPC_Broker.Models;

namespace gRPC_Broker.Repositories;


public interface IPublisherRepository
{
    Task<ArticleModel> AddAnArticle(string userName, Article article);

    Task<bool> PublisherExists(Credentials userModel);

    Task CreateUser(Credentials credentials);

    Task AssignArticleToBeSend(string articleId, List<string> subscribers);


}