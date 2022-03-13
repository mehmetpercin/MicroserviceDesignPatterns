using MassTransit;
using Saga.Orchestration.Payment.API.Consumers;
using Saga.Orchestration.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<StockReservedRequestPaymentEventConsumer>();

    x.UsingRabbitMq((context, configuration) =>
    {
        configuration.Host(builder.Configuration.GetConnectionString("RabbitMq"), "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });

        configuration.ReceiveEndpoint(RabbitMqSettingsConstants.PaymentStockReservedRequestQueueName, e =>
        {
            e.ConfigureConsumer<StockReservedRequestPaymentEventConsumer>(context);
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
