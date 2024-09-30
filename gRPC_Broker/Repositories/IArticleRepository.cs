using gRPC_Broker.Models;

namespace gRPC_Broker.Repositories;

public interface IArticleRepository
{
    Task<ArticleModel> CreateArticle(ArticleModel article);

    Task<List<ArticleModel>> GetUnsentArticles(UserModel user);

    Task CancelSending(UserModel user, List<ArticleModel> articles);
}