using Definitions;
using Grpc.Core;
using gRPC_Broker.Repositories;
using StatusCode = Definitions.StatusCode;
using gRPC_Broker_Publisher;
using Brocker.Exceptions;
using gRPC_Broker.Models;

namespace gRPC_Broker.Services
{
    public class PublisherService : Publisher.PublisherBase
    {
        private IPublisherRepository _publisherRepository;
        private ITopicRepository _topicRepository;
        
        public PublisherService(IPublisherRepository publisherRepository, ITopicRepository topicRepository)
        {
            _publisherRepository = publisherRepository;
            _topicRepository = topicRepository;
        }
        
        public override async Task<Response> CreateUser(Credentials request, ServerCallContext context)
        {
            try
            {
                await _publisherRepository.CreateUser(request);
                return new Response()
                {
                    StatusCode = StatusCode.Success,
                    Message = "User was created"
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    StatusCode = StatusCode.BadRequest,
                    Message = ex.Message
                };
            }
            
        }

        public override async Task<Response> PublishAnArticle(PublishArticleRequest request, ServerCallContext context)
        {
            var article = request.Article;
            var credentials = request.Credentials;
            try
            {
                var publisherExists = await _publisherRepository.PublisherExists(credentials);
                
                if (!publisherExists)
                    throw new PermissionException();


                var insertedArticle = await _publisherRepository.AddAnArticle(credentials.UserName, article);

                if (insertedArticle is null)
                    throw new Exception("Something went wrong saving article");

                var topicSubscribers = await _topicRepository.GetSubscribers(TopicModel.GetTopicModelFromTopicMessage( article.Topic));

                Console.WriteLine("Subscribers:");

                foreach ( var i in topicSubscribers)
                {
                    Console.WriteLine(i);
                }

                await _publisherRepository.AssignArticleToBeSend(insertedArticle.Id, topicSubscribers);

                return new Response()
                {
                    StatusCode = StatusCode.Success,
                    Message = "The article was added"
                };
            }
            catch (UnauthorizedException ex) {
                return new Response() {
                    StatusCode = StatusCode.Unauthorized,
                    Message = ex.Message 
                };
            }
            catch (PermissionException ex) {
                return new Response() {
                    StatusCode = StatusCode.Forbidden,
                    Message = ex.Message 
                };
            }
            catch (Exception ex) {
                return new Response() {
                    StatusCode = StatusCode.BadRequest,
                    Message = ex.Message 
                };
            }
        }
    }
}
