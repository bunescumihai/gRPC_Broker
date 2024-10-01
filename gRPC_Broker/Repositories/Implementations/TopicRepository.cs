using gRPC_Broker.DbContexts;
using gRPC_Broker.Models;
using gRPC_Broker.Models.Projections;
using MongoDB.Driver;

namespace gRPC_Broker.Repositories.Implementations
{
    public class TopicRepository : ITopicRepository
    {
        private MongoDbContext _dbContext;
        public TopicRepository(MongoDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task<List<string>> GetSubscribers(TopicModel topic)
        {
            var projection = Builders<TopicModel>.Projection.Include(t => t.SubscribedUsers);
            
            var filter = Builders<TopicModel>.Filter.Eq(t => t.Name, topic.Name);

            var t = await _dbContext.Topics
                .Find(filter)
                .Project<TopicModel>(projection) 
                .FirstOrDefaultAsync();


            return t.SubscribedUsers.ToList();

        }

        public async Task<List<TopicProjection>>  GetTopics() {
            var topics = await _dbContext.Topics
                .Find(FilterDefinition<TopicModel>.Empty)
                .Project<TopicProjection>(Builders<TopicModel>.Projection
                .Include(p => p.Id)
                .Include(p => p.Name))
                .ToListAsync();

            return topics;
        }
    }
}
