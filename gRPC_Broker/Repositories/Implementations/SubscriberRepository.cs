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

        public async Task UnsubscribeFromTopic(string userName, Topic topic)
        {
            var filter = Builders<TopicModel>.Filter.Eq(t => t.Name, topic.Name);
            var update = Builders<TopicModel>.Update.Pull(t => t.SubscribedUsers, userName);

            var result = await _dbContext.Topics.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                throw new BadTopicException();
        }

        public async Task CreateToSendInstance(string userName)
        {
            await _dbContext.ToSend.InsertOneAsync(new ToSendModel() { UserName = userName });
        }


        public async Task<List<Article>> GetArticlesToSend(string userName)
        {
            var filter = Builders<ToSendModel>.Filter.Eq(t => t.UserName, userName);
            var projection = Builders<ToSendModel>.Projection.Include(t => t.Articles);



            var t = await _dbContext.ToSend.Find(filter).Project<ToSendModel>(projection).FirstOrDefaultAsync();

            var articlesId = t.Articles;

            var filter2 = Builders<ArticleModel>.Filter.In(a => a.Id, articlesId);
            var projection2 = Builders<ArticleModel>.Projection.Include(a => a.TopicModel).Include(a => a.Content);

            var articles = _dbContext.Articles.Find(filter2).ToList();

            foreach (var i in articles)
            {
                Console.WriteLine("----Article----");
                Console.WriteLine("Topic: " + i.TopicModel.Name);
                Console.WriteLine("Topic: " + i.Content);
            }

            List<Article> articles1 = new List<Article>();

            foreach (var article in articles)
                articles1.Add(ArticleModel.GetArticleMessageFromArticleModel(article));

            return articles1;
        }

        public async Task DeleteArticlesToSend(string userName)
        {
            var filter = Builders<ToSendModel>.Filter.Eq(t => t.UserName, userName);
            var update = Builders<ToSendModel>.Update.Set(t => t.Articles, new List<string>());

            try
            {
                await _dbContext.ToSend.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wron while deleting what to send to: " +  ex.Message);
            }

        }
    }
}
