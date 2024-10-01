using Brocker.Exceptions;
using Definitions;
using Grpc.Core;
using gRPC_Broker.Repositories;
using gRPC_Broker.Repositories.Implementations;
using gRPC_Broker_Subscriber;
using MongoDB.Driver.Core.WireProtocol.Messages;
using StatusCode = Definitions.StatusCode;

namespace gRPC_Broker.Services {

    public class SubscriberService : Subscriber.SubscriberBase
    {

        private ISubscriberRepository _subscriberRepository;

        public SubscriberService(ISubscriberRepository subscriberRepository)
        {
            _subscriberRepository = subscriberRepository;
        }

        public override async Task<Response> CreateUser(Credentials request, ServerCallContext context)
        {
            try
            {
                await _subscriberRepository.CreateUser(request);

                await _subscriberRepository.CreateToSendInstance(request.UserName);

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

        public override async Task<Response> SubscribeToTopic(SubscribeUnsubscribeTopicRequest request, ServerCallContext context)
        {

            try
            {
                var userExists = await _subscriberRepository.SubscriberExists(request.Credentials);
                
                if (!userExists)
                    throw new  PermissionException();
                
                await _subscriberRepository.SubscribeToTopic(request.Credentials.UserName, request.Topic);

                return new Response()
                {
                    StatusCode = StatusCode.Success,
                    Message = "You was subscribed to topic: " + request.Topic.Name
                };
            }
            catch (PermissionException ex)
            {
                return new Response()
                {
                    StatusCode = StatusCode.Forbidden,
                    Message = ex.Message
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
    
        public override async Task<Response> UnsubscribeFromTopic(SubscribeUnsubscribeTopicRequest request, ServerCallContext context)
        {

            try
            {
                var userExists = await _subscriberRepository.SubscriberExists(request.Credentials);
                
                if (!userExists)
                    throw new  PermissionException();
                
                await _subscriberRepository.UnsubscribeFromTopic(request.Credentials.UserName, request.Topic);

                return new Response()
                {
                    StatusCode = StatusCode.Success,
                    Message = "You was unsubscribed from topic: " + request.Topic.Name
                };
            }
            catch (PermissionException ex)
            {
                return new Response()
                {
                    StatusCode = StatusCode.Forbidden,
                    Message = ex.Message
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
        
        public override async Task StartGettingArticles(Credentials request, IServerStreamWriter<Response> responseStream, ServerCallContext context)
        {

            try
            {

                var user = await _subscriberRepository.SubscriberExists(request);

                if (!user)
                    throw new PermissionException();


                while(true)
                {
                    if (context.CancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Client disconnected. Streaming stopped.");
                        break;
                    }

                    var articles = await _subscriberRepository.GetArticlesToSend(request.UserName);
                    if(articles != null && articles.Count > 0)
                    {
                        var rs = new Response
                        {
                            StatusCode = StatusCode.Success,
                            Articles = new ArticleList() { Articles = {articles} }
                        };
                        await responseStream.WriteAsync(rs);

                    }

                    await _subscriberRepository.DeleteArticlesToSend(request.UserName);

                    await Task.Delay(5000); 
                }
            }
            catch (RpcException ex) 
            {
                Console.WriteLine("Streaming was cancelled by the client.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    
    }

}
