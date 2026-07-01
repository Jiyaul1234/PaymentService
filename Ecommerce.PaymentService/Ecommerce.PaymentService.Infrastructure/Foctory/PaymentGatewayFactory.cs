using Ecommerce.PaymentService.Application.Interface;
using Ecommerce.PaymentService.Infrastructure.Payment;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.PaymentService.Infrastructure.Foctory
{
    public class PaymentGatewayFactory : IPaymentGatewayFactory
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

                "paypal" => _serviceProvider.GetRequiredService<PayPalPaymentGateway>(),

                _ => throw new Exception("Unsupported gateway")
            };
        }
    }
}

