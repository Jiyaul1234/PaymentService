using AutoMapper;
using Azure.Messaging.ServiceBus;
using Ecommerce.PaymentService.Application.Interface;
using Ecommerce.PaymentService.Application.Interface.IRepository;
using Ecommerce.PaymentService.Application.Interface.IServices;
using Ecommerce.PaymentService.Application.Mapping;
using Ecommerce.PaymentService.Application.Services;
using Ecommerce.PaymentService.Infrastructure;
using Ecommerce.PaymentService.Infrastructure.BusService;
using Ecommerce.PaymentService.Infrastructure.Foctory;
using Ecommerce.PaymentService.Infrastructure.Payment;
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

// AutoMapper
builder.Services.AddAutoMapper(opt => opt.AddProfile<MappingProfile>());

// Repositories and services
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Http client factory for payment gateways
builder.Services.AddHttpClient();

// Register concrete payment gateway implementations so the factory can resolve them by type
builder.Services.AddScoped<StripePaymentGateway>();
builder.Services.AddScoped<PayPalPaymentGateway>();

// Register the factory and producer implementations
builder.Services.AddScoped<IPaymentGatewayFactory, PaymentGatewayFactory>();


// Register Kafka consumer as hosted service (it depends on IPaymentService)
builder.Services.AddHostedService<KafkaConsumer>();

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
