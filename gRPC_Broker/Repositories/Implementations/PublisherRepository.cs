using Brocker.Exceptions;
using Definitions;
using gRPC_Broker.DbContexts;
using gRPC_Broker.Models;
using MongoDB.Driver;
using UserRole = gRPC_Broker.Definitions.UserRole;


namespace gRPC_Broker.Repositories.Implementations
{
    public class PublisherRepository : IPublisherRepository
    {
        private MongoDbContext _dbContext;

        public PublisherRepository(MongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateUser(global::Definitions.Credentials credentials)
        {

            var filter = Builders<UserModel>.Filter.Eq(u => u.UserName, credentials.UserName);
            
            UserModel userModel = await _dbContext.Users.Find(filter).FirstOrDefaultAsync();

            if ( userModel is not null)
                throw new UserExistsException();

            UserModel user = new UserModel() { UserName = credentials.UserName, Password = credentials.Password, Role = UserRole.Publisher };
            
            _dbContext.Users.InsertOne(user);
        }

        public async Task<bool> PublisherExists(global::Definitions.Credentials credentials)
        {
            var filter = Builders<UserModel>.Filter.And(
                Builders<UserModel>.Filter.Eq(u => u.UserName, credentials.UserName),
                Builders<UserModel>.Filter.Eq(u => u.Password, credentials.Password),
                Builders<UserModel>.Filter.Eq(u => u.Role, UserRole.Publisher)
            );

            UserModel userModel = await _dbContext.Users.Find(filter).FirstOrDefaultAsync();

            if (userModel is not null)
                return true;

            return false;
        }

        public async Task<ArticleModel> AddAnArticle(string userName, Article article)
        {
            var a = ArticleModel.GetArticleModelFromArticleMessage(userName, article);
            await _dbContext.Articles.InsertOneAsync(a);
            return a; 
        }

        public async Task AssignArticleToBeSend(string articleId, List<string> subscribers)
        {
            var filter = Builders<ToSendModel>.Filter.In(t => t.UserName, subscribers);

            var update = Builders<ToSendModel>.Update
                .AddToSet(t => t.Articles, articleId); 

            var updateResult = await _dbContext.ToSend.UpdateManyAsync(filter, update);

            Console.WriteLine($"{updateResult.ModifiedCount} users updated with article {articleId}");

        }

    }
}
