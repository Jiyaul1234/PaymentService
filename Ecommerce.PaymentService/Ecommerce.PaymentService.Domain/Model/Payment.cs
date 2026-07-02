using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ecommerce.PaymentService.Domain.Model
{
    [Table("Payment")]
    public  class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        public  int OrderId { get; set; }

        public string? CustomerId { get; set; }

        public decimal Amount { get; set; }

        public string? Currency { get; set; }

        public string ?PaymentMethod { get; set; }

        public string? PaymentStatus { get; set; }

        public string? TransactionId { get; set; }

        public string? PaymentGatewayId { get; set; }

        public string ?PaymentGatewayResponse { get; set; }

        public string? PaymentGatewayError { get; set; }


        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
