using MassTransit;
using Microsoft.EntityFrameworkCore;
using Saga.Orchestration.Shared;
using Saga.Orchestration.Stock.API.Consumers;
using Saga.Orchestration.Stock.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseInMemoryDatabase("StockDb");
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedEventConsumer>();
    x.AddConsumer<StockRollBackMessageConsumer>();

    x.UsingRabbitMq((context, configuration) =>
    {
        configuration.Host(builder.Configuration.GetConnectionString("RabbitMq"), "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });

        configuration.ReceiveEndpoint(RabbitMqSettingsConstants.StockOrderCreatedEventQueueName, e =>
        {
            e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
        });

        configuration.ReceiveEndpoint(RabbitMqSettingsConstants.StockRollBackMessageQueueName, e =>
        {
            e.ConfigureConsumer<StockRollBackMessageConsumer>(context);
        });
    });
});
builder.Services.AddMassTransitHostedService();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
    dbContext.Stocks.Add(new Stock
    {
        Id = 1,
        ProductId = 1,
        Quantity = 100
    });
    dbContext.Stocks.Add(new Stock
    {
        Id = 2,
        ProductId = 2,
        Quantity = 200
    });
    dbContext.Stocks.Add(new Stock
    {
        Id = 3,
        ProductId = 3,
        Quantity = 300
    });

    dbContext.SaveChanges();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
