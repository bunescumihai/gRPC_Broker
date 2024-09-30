using gRPC_Broker.Models;
using gRPC_Broker.Definitions;
using gRPC_Broker.DbContexts;

namespace gRPC_Broker.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {

        private MongoDbContext _dbContext;

        public UserRepository(MongoDbContext dbContext)
        {
            _dbContext = dbContext; 
        }
    
        public Task<List<ArticleModel>> GetSenderArticles(UserModel user)
        {
            throw new NotImplementedException();
        }

        public async Task<UserModel?> GetUser(string username, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<UserModel> RegisterAsPublisher(string username, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<UserModel> RegisterAsSubscriber(string username, string password)
        {
            var user = new UserModel() { UserName = username, Password = password, Role = UserRole.Subscriber };

            await _dbContext.Users.InsertOneAsync(user);
            return user;
        }
    }
}
