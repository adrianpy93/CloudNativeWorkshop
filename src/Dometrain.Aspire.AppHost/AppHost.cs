var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("main-db", port: 5433);
//.WithLifetime(ContainerLifetime.Persistent)

// Add init script to create database
postgres.WithBindMount("./postgres-init", "/docker-entrypoint-initdb.d");

var mainDb = postgres.AddDatabase("dometrain");

var redis = builder.AddRedis("redis");
//.WithLifetime(ContainerLifetime.Persistent)

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    //.WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin();

builder.AddProject("dometrain-api", "../Dometrain.Monolith.Api/Dometrain.Monolith.Api.csproj")
    //.WithReplicas(5)
    .WithReference(mainDb)
    .WaitFor(mainDb)
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(rabbitmq);

var app = builder.Build();

await app.RunAsync();