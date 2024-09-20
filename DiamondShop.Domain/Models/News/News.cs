using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.News.Enum;
using DiamondShop.Domain.Models.News.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.News
{
    public class News : Entity<NewsId>, IAggregateRoot
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public PriorityLevel PriorityLevel { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
