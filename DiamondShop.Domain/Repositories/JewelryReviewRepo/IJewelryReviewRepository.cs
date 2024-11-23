using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;

namespace DiamondShop.Domain.Repositories.JewelryReviewRepo
{
    public interface IJewelryReviewRepository : IBaseRepository<JewelryReview>
    {
        Task<bool> Exists(JewelryId id);
    }
}
