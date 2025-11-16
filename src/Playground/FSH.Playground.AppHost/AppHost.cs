var builder = DistributedApplication.CreateBuilder(args);

// Postgres container + database
var postgres = builder.AddPostgres("postgres").WithDataVolume("fsh-postgres-data").AddDatabase("fsh");

var redis = builder.AddRedis("redis").WithDataVolume("fsh-redis-data");

builder.AddProject<Projects.Playground_Api>("playground-api")
    .WithReference(postgres)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Production")
    .WithEnvironment("DatabaseOptions__Provider", "POSTGRESQL")
    .WithEnvironment("DatabaseOptions__ConnectionString", postgres.Resource.ConnectionStringExpression)
    .WithEnvironment("DatabaseOptions__MigrationsAssembly", "FSH.Playground.Migrations.PostgreSQL")
    .WaitFor(postgres)
    .WithReference(redis)
    .WithEnvironment("CachingOptions__Redis", redis.Resource.ConnectionStringExpression)
    .WaitFor(redis);

await builder.Build().RunAsync();
