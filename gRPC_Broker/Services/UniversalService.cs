using Definitions;
using Grpc.Core;
using gRPC_Broker.Models;
using gRPC_Broker.Repositories;
using gRPC_Broker_Universal;
using Empty = Definitions.Empty;
using StatusCode = Definitions.StatusCode;


namespace gRPC_Broker.Services
{
    public class UniversalService : Universal.UniversalBase
    {

        private readonly ITopicRepository _topicRepository;

        public UniversalService(ITopicRepository topicRepository) {
            _topicRepository = topicRepository;
        }

        public override async Task<Response> GetTopics(Empty request, ServerCallContext context)
        {
            var topics = await _topicRepository.GetTopics(); // Fetch the list of TopicModel

            var tp = new List<Topic>();
            
            topics.ForEach(topic =>
            {
                tp.Add(new Topic() { Name = topic.Name });
            });
            
            return new Response()
            {
                StatusCode = StatusCode.Success,
                Topics = new TopicList() {Topics = { tp }}
            };
        }
    }
}
