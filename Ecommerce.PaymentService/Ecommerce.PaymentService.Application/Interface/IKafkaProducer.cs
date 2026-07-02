using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.PaymentService.Application.Interface
{
    public interface IKafkaProducer
    {
        public Task PublishAsync<T>(string topic, T message);
    }
}
