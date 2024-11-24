using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Common.Products
{
    public record ProductLock
    {
        public AccountId? AccountId { get; set; }
        [NotMapped]
        public Account? Account { get; set; }
        public DateTime? LockEndDate { get; set; }   
        public static ProductLock CreateLockForUser(AccountId accountId, TimeSpan howlong)
        {
            return new ProductLock
            {
                AccountId = accountId,
                LockEndDate = DateTime.UtcNow.Add(howlong)
            };
        }
    }
}
