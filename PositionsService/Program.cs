using PositionsService;
using PositionsService.EventBus;
using PositionsService.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<PositionService>(); // Registers PositionService
builder.Services.AddSingleton<RateChangeListener>();
builder.Services.AddHostedService<PositionsWorker>();

var host = builder.Build();
host.Run();