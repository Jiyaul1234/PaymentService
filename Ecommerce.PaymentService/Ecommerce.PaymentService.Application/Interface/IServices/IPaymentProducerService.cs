using Ecommerce.PaymentService.Application.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.PaymentService.Application.Interface.IServices
{
    public interface IPaymentProducerService
    {
        public Task PublishPaymentStatus(PaymentCreatedEvent paymentCreatedEvent);
    }
}
