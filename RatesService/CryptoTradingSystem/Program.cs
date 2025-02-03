using RatesService.Services;
using RatesService.EventBus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Register HttpClient for RateFetcherService (dependency injection)
builder.Services.AddHttpClient<RateFetcherService>(); // Calls constructor in RateFetcherService
builder.Services.AddSingleton<IEventBus, RabbitMQProducer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
