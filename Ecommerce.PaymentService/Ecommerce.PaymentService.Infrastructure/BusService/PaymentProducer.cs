using Azure.Messaging.ServiceBus;
using Ecommerce.PaymentService.Application.Events;
using Ecommerce.PaymentService.Application.Interface.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Ecommerce.PaymentService.Infrastructure.BusService
{
    public class PaymentProducer : IPaymentProducerService
    {
        private readonly ServiceBusSender _sender;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentProducer> _logger;

        public PaymentProducer(ServiceBusClient client, IConfiguration configuration, ILogger<PaymentProducer> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _sender = client.CreateSender(configuration["AzureServiceBus:QueueName1"]);
        }
        public async Task PublishPaymentStatus(PaymentCreatedEvent paymentCreatedEvent)
        {

            _logger.LogInformation($"Start publish PaymentCreatedEvent:{JsonSerializer.Serialize(paymentCreatedEvent)}");
            var json = JsonSerializer.Serialize(paymentCreatedEvent);

            var message = new ServiceBusMessage(json)
            {
                MessageId = paymentCreatedEvent.MessageId,
                ContentType = "application/json"
            };

            message.ApplicationProperties.Add("EventType", "PaymentCreated");

            await _sender.SendMessageAsync(message);

            _logger.LogInformation($"Published successfully paymentId:{paymentCreatedEvent.PaymentId}");
        }
    }
}
