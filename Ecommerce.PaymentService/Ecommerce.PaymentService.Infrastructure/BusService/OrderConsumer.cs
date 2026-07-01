using Azure.Messaging.ServiceBus;
using Ecommerce.PaymentService.Application.Dto;
using Ecommerce.PaymentService.Application.Events;
using Ecommerce.PaymentService.Application.Interface;
using Ecommerce.PaymentService.Application.Interface.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

public class OrderConsumer : IHostedService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IConfiguration _configuration;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<OrderConsumer> _logger;
    private readonly IPaymentGateway  _paymentGateway;

    public OrderConsumer(
        ServiceBusClient client,
        IConfiguration configuration, IPaymentService paymentService, ILogger<OrderConsumer> logger,IPaymentGateway paymentGateway)
    {
        _configuration = configuration;
        _paymentService= paymentService;
        _paymentGateway= paymentGateway;
        _logger = logger;
        _processor = client.CreateProcessor(
            configuration["AzureServiceBus:QueueName"],
            new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 5
            });
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting OrderConsumer...");
        _processor.ProcessMessageAsync += ProcessMessage;
        _processor.ProcessErrorAsync += ErrorHandler;

        await _processor.StartProcessingAsync();
        _logger.LogInformation("Consumed message.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync();
    }

    private async Task ProcessMessage(ProcessMessageEventArgs args)
    {
        try
        {
            _logger.LogInformation("Processing message: {MessageId}", args.Message.MessageId);
            var body = args.Message.Body.ToString();

            var order =
                JsonSerializer.Deserialize<OrderCreatedEvent>(body);

            // Business Logic
            PaymentDto paymentDto = new PaymentDto
            {
                Amount = order.Amount,
                OrderId = order.OrderId,
                MessageId = order.MessageId,
                CreatedDate = order.CreatedOn,
                PaymentMethod = order.PaymentMethod,   
               
            };

             await _paymentService.CreatePaymentAsync(paymentDto);

            _logger.LogInformation("Processed message: {MessageId}", args.Message.MessageId);   

            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

            await args.AbandonMessageAsync(args.Message);
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception);

        return Task.CompletedTask;
    }
}