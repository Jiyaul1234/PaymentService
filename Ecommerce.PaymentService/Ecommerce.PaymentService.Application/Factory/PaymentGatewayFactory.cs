using Ecommerce.PaymentService.Application.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.PaymentService.Application.Factory
{
    public class PaymentGatewayFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PaymentGatewayFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPaymentGateway GetGateway(string gateway)
        {
            return gateway.ToLower() switch
            {
                "stripe" => _serviceProvider.GetRequiredService<StripePaymentGateway>(),

                "paypal" => _serviceProvider.GetRequiredService<PaypalPaymentGateway>(),

                _ => throw new Exception("Unsupported gateway")
            };
        }
    }
}
