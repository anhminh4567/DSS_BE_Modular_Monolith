using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Blogs.Entities;
using DiamondShop.Domain.Models.Blogs.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamondShop.Domain.Models.Blogs
{
    public class Blog : Entity<BlogId>
    {
        public List<BlogTag> Tags { get; set; } = new();
        public AccountId AccountId { get; set; }
        public Account Account { get; set; }
        public Media? Thumbnail { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        [NotMapped]
        public string Content { get; set; }
        private Blog() { }
        public static Blog Create(List<BlogTag> blogTags, AccountId accountId, string title)
        {
            return new Blog()
            {
                Id = BlogId.Create(),
                Tags = blogTags,
                AccountId = accountId,
                Title = title,
                CreatedDate = DateTime.UtcNow,
            };
        }
    }
}
