using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryReviewRepo;

namespace DiamondShop.Infrastructure.Databases.Repositories.JewelryRepo.JewelryReviewRepo
{
    internal class JewelryReviewRepository : BaseRepository<JewelryReview> , IJewelryReviewRepository
    {
        public JewelryReviewRepository(DiamondShopDbContext context) : base(context) { }
        public override async Task<JewelryReview?> GetById(params object[] ids)
        {
            var id = (JewelryReviewId)ids[0];
            return await base.GetById(id);
        }
    }
}
