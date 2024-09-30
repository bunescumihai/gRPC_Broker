using Definitions;
using Grpc.Core;
using gRPC_Broker.Repositories;
using StatusCode = Definitions.StatusCode;
using gRPC_Broker_Publisher;
using Brocker.Exceptions;

namespace gRPC_Broker.Services
{
    public class PublisherService : Publisher.PublisherBase
    {
        private IPublisherRepository _publisherRepository;
        
        public PublisherService(IPublisherRepository publisherRepository)
        {
            _publisherRepository = publisherRepository;
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
                await _publisherRepository.AddAnArticle(credentials, article);
                return new Response()
                {
                    StatusCode = StatusCode.Unauthorized,
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
