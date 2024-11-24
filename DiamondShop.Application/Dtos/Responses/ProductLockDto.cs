using DiamondShop.Application.Dtos.Responses.Accounts;
using DiamondShop.Domain.Common.Products;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses
{
    public class ProductLockDto
    {
        public string? AccountId { get; set; }
        public string? LockEndDate { get; set; }
        public AccountDto? Account { get; set; }
    }
}
