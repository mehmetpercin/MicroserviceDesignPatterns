using Polly;
using Polly.Extensions.Http;
using Retry.ServiceA;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient<ProductService>(opt =>
{
    opt.BaseAddress = new Uri("http://localhost:5002/api/products/");
}).AddPolicyHandler(GetAdvancedCircuitBreakerPolicy());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions.HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound).WaitAndRetryAsync(5, retryAttemp =>
        {
            Debug.WriteLine($"Retry Count : {retryAttemp}");
            return TimeSpan.FromSeconds(10);
        }, onRetryAsync: OnRetryAsync);
}

IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions.HandleTransientHttpError()
        .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10), onBreak: (arg1, arg2) => 
        {
            Debug.WriteLine($"Circuit breaker status : On Break");
        }, onReset: () =>
        {
            Debug.WriteLine($"Circuit breaker status : On Reset");
        }, onHalfOpen: () =>
        {
            Debug.WriteLine($"Circuit breaker status : On Half Open");
        });
}

IAsyncPolicy<HttpResponseMessage> GetAdvancedCircuitBreakerPolicy()
{
    return HttpPolicyExtensions.HandleTransientHttpError()
        .AdvancedCircuitBreakerAsync(0.5 /* %50 */, TimeSpan.FromSeconds(10), 5, TimeSpan.FromSeconds(60), onBreak: (arg1, arg2) =>
        {
            Debug.WriteLine($"Circuit breaker status : On Break");
        }, onReset: () =>
        {
            Debug.WriteLine($"Circuit breaker status : On Reset");
        }, onHalfOpen: () =>
        {
            Debug.WriteLine($"Circuit breaker status : On Half Open");
        });
}

Task OnRetryAsync(DelegateResult<HttpResponseMessage> arg1, TimeSpan arg2)
{
    Debug.WriteLine($"Request made again. Total time : {arg2.TotalMilliseconds}");
    return Task.CompletedTask;
}

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
