using gRPC_Broker.DbContexts;
using gRPC_Broker.Models;
using gRPC_Broker.Models.Projections;

namespace gRPC_Broker.Repositories
{
    public interface ITopicRepository
    {
        public Task<List<TopicProjection>> GetTopics();
        public Task<List<string>> GetSubscribers(TopicModel topicModel);
    }
}
