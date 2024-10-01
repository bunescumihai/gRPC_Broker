using MongoDB.Driver;
using gRPC_Broker.Models;


namespace gRPC_Broker.DbContexts
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<TopicModel> Topics => _database.GetCollection<TopicModel>("Topics");
        public IMongoCollection<UserModel> Users => _database.GetCollection<UserModel>("Users");
        public IMongoCollection<ArticleModel> Articles => _database.GetCollection<ArticleModel>("Articles");
        public IMongoCollection<ToSendModel> ToSend => _database.GetCollection<ToSendModel>("ToSend");
    }
}
