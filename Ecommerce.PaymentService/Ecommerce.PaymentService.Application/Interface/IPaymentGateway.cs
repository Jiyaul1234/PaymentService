using Ecommerce.PaymentService.Application.Models;

namespace Ecommerce.PaymentService.Application.Interface
{
    public interface IPaymentGateway
    {
        Task<PaymentResult> PayAsync(PaymentRequest request);
    }
}
