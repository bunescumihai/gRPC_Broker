using Brocker.Exceptions;
using Definitions;
using gRPC_Broker.DbContexts;
using gRPC_Broker.Models;
using MongoDB.Driver;
using UserRole = gRPC_Broker.Definitions.UserRole;


namespace gRPC_Broker.Repositories.Implementations
{
    public class SubscriberRepository : ISubscriberRepository
    {

        private MongoDbContext _dbContext;

        public SubscriberRepository(MongoDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task CreateUser(Credentials credentials)
        {

            var filter = Builders<UserModel>.Filter.Eq(u => u.UserName, credentials.UserName);

            UserModel userModel = await _dbContext.Users.Find(filter).FirstOrDefaultAsync();

            if (userModel is not null)
                throw new UserExistsException();

            UserModel user = new UserModel() { UserName = credentials.UserName, Password = credentials.Password, Role = UserRole.Subscriber };

            _dbContext.Users.InsertOne(user);
        }

        public Task<List<ArticleModel>> GetArticles(string userName)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SubscriberExists(Credentials credentials)
        { 

            var filter = Builders<UserModel>.Filter.And(
                    Builders<UserModel>.Filter.Eq(s => s.UserName, credentials.UserName),
                    Builders<UserModel>.Filter.Eq(s => s.Password, credentials.Password),
                    Builders<UserModel>.Filter.Eq(s => s.Role, UserRole.Subscriber)
                );


            return await _dbContext.Users.Find(filter).FirstOrDefaultAsync() is null ? false : true;
        }

        public async Task SubscribeToTopic(string userName, Topic topic)
        {
            var filter = Builders<TopicModel>.Filter.Eq(t => t.Name, topic.Name);
            var update = Builders<TopicModel>.Update.AddToSet(t => t.SubscribedUsers, userName);
            
            var result = await _dbContext.Topics.UpdateOneAsync(filter, update);
            
            if (result.MatchedCount == 0)
                throw new BadTopicException();

        }

        public async Task UnsubscriveFromTopic(string userName, Topic topic)
        {
            var filter = Builders<TopicModel>.Filter.Eq(t => t.Name, topic.Name);
            var update = Builders<TopicModel>.Update.Pull(t => t.SubscribedUsers, userName);

            var result = await _dbContext.Topics.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                throw new BadTopicException();
        }

    }
}
