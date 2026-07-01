using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.PaymentService.Application.Models
{
    public class PaymentRequest
    {
        public int PaymentId { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public string CustomerEmail { get; set; }
    }
}
