using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.PaymentService.Application.Interface
{
    public interface IPaymentGatewayFactory
    {
        public IPaymentGateway GetGateway(string gateway);
    }
}
