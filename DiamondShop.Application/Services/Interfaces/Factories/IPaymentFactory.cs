using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces.Factories
{
    public interface IPaymentFactory
    {
        IPaymentService GetPaymentService(string paygateId);
    }
}
