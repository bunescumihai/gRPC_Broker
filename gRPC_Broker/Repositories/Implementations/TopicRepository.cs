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
