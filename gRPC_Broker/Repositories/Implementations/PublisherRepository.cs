using Brocker.Exceptions;
using Definitions;
using gRPC_Broker.DbContexts;
using gRPC_Broker.Models;
using MongoDB.Driver;
using UserRole = gRPC_Broker.Definitions.UserRole;


namespace gRPC_Broker.Repositories.Implementations
{
    public class PublisherRepository: IPublisherRepository
    {
        private MongoDbContext _dbContext;

        public PublisherRepository(MongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        
        public void SaveArticle()
        {
            throw new NotImplementedException();
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

        public async Task<bool> UserExists(global::Definitions.Credentials credentials)
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

        public async Task AddAnArticle(global::Definitions.Credentials credentials, Article article)
        {
            if (!await UserExists(credentials))
                throw new PermissionException();

            await _dbContext.Articles.InsertOneAsync(ArticleModel.GetArticleModelFromArticleMessage(credentials.UserName, article));

        }
    }
}
