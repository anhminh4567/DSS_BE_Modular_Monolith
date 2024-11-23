using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryReviewRepo;
using Microsoft.EntityFrameworkCore;

namespace DiamondShop.Infrastructure.Databases.Repositories.JewelryRepo.JewelryReviewRepo
{
    internal class JewelryReviewRepository : BaseRepository<JewelryReview> , IJewelryReviewRepository
    {
        public JewelryReviewRepository(DiamondShopDbContext context) : base(context) { }
        public override async Task<JewelryReview?> GetById(params object[] ids)
        {
            var id = (JewelryId)ids[0];
            return await _set.Include(p => p.Jewelry).FirstOrDefaultAsync(p => p.Id == id);
        }
        public Task<bool> Exists(JewelryId id)
        {
            return _set.AnyAsync(p => p.Id == id);
        }

    }
}
