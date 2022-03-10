using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, configuration) =>
    {
        configuration.Host(builder.Configuration.GetConnectionString("RabbitMq"), "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
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
