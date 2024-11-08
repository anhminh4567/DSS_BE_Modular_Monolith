using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamondShop.Domain.Models.Jewelries.Entities
{
    public class JewelryReview : Entity<JewelryId>
    {
        public Jewelry Jewelry { get; set; }
        public AccountId AccountId { get; set; }
        public Account Account { get; set; }
        public string Content { get; set; }
        public int StarRating { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsHidden { get; set; }
        [NotMapped]
        public List<Media>? Medias { get; set; } = new();
        private JewelryReview() { }
        public static JewelryReview Create(JewelryId jewelryId, AccountId accountId, string content, int starRating)
        {
            return new JewelryReview
            {
                Id = jewelryId,
                AccountId = accountId,
                Content = content,
                StarRating = starRating,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                IsHidden = false,
            };
        }
    }
}
