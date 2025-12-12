using Aspire.Hosting.Azure;
using Azure.Provisioning.CosmosDB;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var mainDbUsername = builder.AddParameter("postgres-username");
var mainDbPassword = builder.AddParameter("postgres-password");

var mainDb = builder.AddPostgres("main-db", mainDbUsername, mainDbPassword, port: 5433)
    //.WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("dometrain");

var cartDb = builder.AddAzureCosmosDB("cosmosdb")
    .AddDatabase("cartdb");

var redis = builder.AddRedis("redis");
    //.WithLifetime(ContainerLifetime.Persistent)

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    //.WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin();

builder.AddProject("dometrain-api", "../Dometrain.Monolith.Api/Dometrain.Monolith.Api.csproj")
    //.WithReplicas(5)
    .WithReference(mainDb)
    .WithReference(cartDb)
    .WithReference(redis)
    .WithReference(rabbitmq);

var app = builder.Build();

app.Run();



