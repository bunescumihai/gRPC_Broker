using Brocker.Exceptions;
using Definitions;
using Grpc.Core;
using gRPC_Broker.Repositories;
using gRPC_Broker_Subscriber;
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
                
                await _subscriberRepository.UnsubscriveFromTopic(request.Credentials.UserName, request.Topic);

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
    
    
    }

}
