using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Notifications.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Notifications
{
    public class Notification : Entity<NotificationId>, IAggregateRoot
    {
        public AccountId? AccountId { get; set; }
        public Account? Account { get; set; }
        public OrderId? OrderId { get; set; }
        public Order? Order { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
        [NotMapped]
        public bool IsForShop => AccountId == null;
        [NotMapped]
        public bool IsForOrder => OrderId != null;
        [NotMapped]
        public bool IsPublicMessages => AccountId == null && OrderId == null;
        public Notification() { }

    }
}
