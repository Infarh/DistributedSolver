using DistributedSolver.Host.Workers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ServiceObserverWorker>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
