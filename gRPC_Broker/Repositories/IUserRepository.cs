using gRPC_Broker.Models;

namespace gRPC_Broker.Repositories;

public interface IUserRepository
{
    Task<UserModel?> RegisterAsPublisher(string username, string password);
    Task<UserModel?> RegisterAsSubscriber(string username, string password);
    
    Task<List<ArticleModel>> GetSenderArticles(UserModel user);
    Task<UserModel?> GetUser(string username, string password);
}