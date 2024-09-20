using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Blogs.Entities;
using DiamondShop.Domain.Models.Blogs.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Blogs
{
    public class Blog: Entity<BlogId>
    {
        public List<BlogTag> Tags { get; set; } = new ();
        public AccountId AccountId { get; set; }
        public Account Account { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<BlogMedia> Medias { get; set; } = new ();
    }
}
