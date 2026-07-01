namespace Ecommerce.PaymentService.Application.Models
{
    public class PaymentResult
    {
        public bool IsSuccess { get; set; }

        public string TransactionId { get; set; }

        public string ErrorMessage { get; set; }
    }
}
