using Azure.Messaging.ServiceBus;
using Confluent.Kafka;
using Ecommerce.PaymentService.Application.Events;
using Ecommerce.PaymentService.Application.Interface;
using Ecommerce.PaymentService.Application.Interface.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Ecommerce.PaymentService.Infrastructure.BusService
{
    public class KafkaProducer:IKafkaProducer 
    {
        private readonly IProducer<string, string> _producer;
        public KafkaProducer(IConfiguration configuration)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                Acks = Acks.All,
                EnableIdempotence = true
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task PublishAsync<T>(string topic, T message)
        {
            var json = JsonSerializer.Serialize(message);

            await _producer.ProduceAsync(topic,
                new Message<string, string>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = json
                });
        }
    }
}
