using MassTransit;
using Microsoft.EntityFrameworkCore;
using Saga.Orchestration.Shared;
using Saga.Orchestration.StateMachineWorkerService;
using Saga.Orchestration.StateMachineWorkerService.Models;
using System.Reflection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

        services.AddMassTransit(x =>
        {
            x.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>().EntityFrameworkRepository(opt =>
            {
                opt.ConcurrencyMode = MassTransit.EntityFrameworkCoreIntegration.ConcurrencyMode.Optimistic;
                opt.AddDbContext<DbContext, OrderStateDbContext>((provider, builder) =>
                {
                    builder.UseSqlServer(configuration.GetConnectionString("SqlServer"), m =>
                    {
                        m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                    });
                });
            });

            x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(configure =>
            {
                configure.Host(configuration.GetConnectionString("RabbitMq"), "/", host =>
                {
                    host.Username("guest");
                    host.Password("guest");
                });

                configure.ReceiveEndpoint(RabbitMqSettingsConstants.OrderSaga, e =>
                {
                    e.ConfigureSaga<OrderStateInstance>(provider);
                });
            }));
        });
        services.AddMassTransitHostedService();



        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
