using Definitions;
using Grpc.Net.Client;
using gRPC_Broker_Universal;
using gRPC_Broker_Publisher;
using Microsoft.VisualBasic;
using gRPC_Broker_Subscriber;

using var channel = GrpcChannel.ForAddress("http://192.168.1.5:8143");

var universalClient = new Universal.UniversalClient(channel);
var publisherClient = new Publisher.PublisherClient(channel);
var subscriberClient = new Subscriber.SubscriberClient(channel);


var data = universalClient.GetTopics(new Empty());



foreach (var topic in data.Topics.Topics)
{
    Console.WriteLine(topic.Name);
}

data = subscriberClient.CreateUser(new Credentials() { UserName = "Vesnuska", Password = "Vesnuska" });


Console.WriteLine(data.Message);

data = subscriberClient.SubscribeToTopic(
    new SubscribeUnsubscribeTopicRequest()
    {
        Credentials = new Credentials()
        {
            UserName = "valera",
            Password = "valera"
        },
        Topic = new Topic()
        {
            Name = "Music"
        }
    });

data = subscriberClient.UnsubscribeFromTopic(
    new SubscribeUnsubscribeTopicRequest()
    {
        Credentials = new Credentials()
        {
            UserName = "Vesnuska",
            Password = "Vesnuska"
        },
        Topic = new Topic()
        {
            Name = "Music"
        }
    });

Console.WriteLine(data.Message);


Console.ReadLine();