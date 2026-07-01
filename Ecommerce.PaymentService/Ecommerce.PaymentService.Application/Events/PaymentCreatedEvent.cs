using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.PaymentService.Application.Events
{
    public class PaymentCreatedEvent
    {
        public string MessageId { get; set; }
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        public string Status { get; set; }

    }
}
