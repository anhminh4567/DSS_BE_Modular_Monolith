using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces
{
    public interface IPdfService
    {
        string GetTemplateHtmlStringFromOrder(Order order, Account customerAccount);
        Stream ParseHtmlToPdf(string htmlString);
    }
}
