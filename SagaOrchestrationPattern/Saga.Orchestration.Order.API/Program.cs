using MassTransit;
using Microsoft.EntityFrameworkCore;
using Saga.Orchestration.Order.API.Consumers;
using Saga.Orchestration.Shared;
using Saga.Orchestrication.Order.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCompletedRequestEventConsumer>();
    x.AddConsumer<OrderFailedRequestEventConsumer>();

    x.UsingRabbitMq((context, configuration) =>
    {
        configuration.Host(builder.Configuration.GetConnectionString("RabbitMq"), "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });

        configuration.ReceiveEndpoint(RabbitMqSettingsConstants.OrderCompletedRequestQueueName, e =>
        {
            e.ConfigureConsumer<OrderCompletedRequestEventConsumer>(context);
        });

        configuration.ReceiveEndpoint(RabbitMqSettingsConstants.OrderFailedRequestQueueName, e =>
        {
            e.ConfigureConsumer<OrderFailedRequestEventConsumer>(context);
        });
    });
});
builder.Services.AddMassTransitHostedService();


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

app.UseAuthorization();

app.MapControllers();

app.Run();
