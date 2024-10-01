using gRPC_Broker.DbContexts;
using gRPC_Broker.Repositories;
using gRPC_Broker.Repositories.Implementations;
using gRPC_Broker.Services;
using System.Net;


string ip = "192.168.0.50";
//string ip = "172.20.10.3";
int port = 8143;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddSingleton<MongoDbContext>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDbConnection");
    return new MongoDbContext(connectionString, "no_name_db");
});

builder.Services.AddScoped<ITopicRepository, TopicRepository>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<ISubscriberRepository, SubscriberRepository>();


builder.WebHost.ConfigureKestrel(options => { options.Listen(IPAddress.Parse(ip), port); });

var app = builder.Build();


app.MapGrpcService<UniversalService>();
app.MapGrpcService<SubscriberService>();
app.MapGrpcService<PublisherService>();


app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();