using Ecommerce.PaymentService.Application.Interface;
using Ecommerce.PaymentService.Application.Models;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Ecommerce.PaymentService.Infrastructure.Payment
{
    public class StripePaymentGateway : IPaymentGateway
    {
        public StripePaymentGateway(IConfiguration configuration)
        {
            StripeConfiguration.ApiKey =
                configuration["Stripe:SecretKey"];
        }

        public async Task<PaymentResult> PayAsync(PaymentRequest request)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(request.Amount * 100), // cents
                Currency = request.Currency.ToLower(),
                AutomaticPaymentMethods =
                    new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true
                    }
            };

            var service = new PaymentIntentService();

            var intent = await service.CreateAsync(options);

            return new PaymentResult
            {
                IsSuccess = true,
                TransactionId = intent.Id
            };
        }
    }
}
