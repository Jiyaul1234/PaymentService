namespace Ecommerce.PaymentService.Application.Events
{
    public class OrderCreatedEvent
    {
        public string MessageId { get; set; }

        public int OrderId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; }

        public DateTime CreatedOn { get; set; }

    }
}
