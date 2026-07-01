using AutoMapper;
using Azure.Messaging.ServiceBus;
using Ecommerce.PaymentService.Application.Interface.IRepository;
using Ecommerce.PaymentService.Application.Interface.IServices;
using Ecommerce.PaymentService.Application.Services;
using Ecommerce.PaymentService.Infrastructure;
using Ecommerce.PaymentService.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DbConnetion");
    options.UseSqlServer(connectionString);
});


builder.Services.AddSingleton(x =>
{
    var configuration = x.GetRequiredService<IConfiguration>();

    return new ServiceBusClient(
        configuration["AzureServiceBus:ConnectionString"],
        new ServiceBusClientOptions
        {
            RetryOptions =
            {
                MaxRetries = 5,
                Delay = TimeSpan.FromSeconds(2),
                Mode = ServiceBusRetryMode.Exponential
            }
        });
});
// AutoMapper
builder.Services.AddAutoMapper(typeof(Ecommerce.PaymentService.Application.Mapping.MappingProfile));

// Repositories and services
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Http client factory for payment gateways
builder.Services.AddHttpClient();

// Register payment gateways
builder.Services.AddScoped<Ecommerce.PaymentService.Application.Interface.IPaymentGateway, Ecommerce.PaymentService.Infrastructure.Payment.StripePaymentGateway>();
builder.Services.AddScoped<Ecommerce.PaymentService.Application.Interface.IPaymentGateway, Ecommerce.PaymentService.Infrastructure.Payment.PayPalPaymentGateway>();
builder.Services.AddHostedService<OrderConsumer>();

// If AppDbContext is not EF Core DbContext, you may need to register it differently.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
