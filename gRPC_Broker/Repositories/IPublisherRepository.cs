
using Definitions;

namespace gRPC_Broker.Repositories;


public interface IPublisherRepository
{
    Task AddAnArticle(Credentials credentials, Article article);

    Task<bool> UserExists(Credentials userModel);

    Task CreateUser(Credentials credentials);
}