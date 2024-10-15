using DiamondShop.Domain.Models.DeliveryFees;
using DiamondShop.Domain.Models.DeliveryFees.ValueObjects;
using DiamondShop.Domain.Repositories.DeliveryRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories.DeliveryRepo
{
    internal class DeliveryFeeRepository : BaseRepository<DeliveryFee>, IDeliveryFeeRepository
    {
        public DeliveryFeeRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }

        public Task CreateRange(List<DeliveryFee> fees, CancellationToken cancellationToken = default)
        {
            return _set.AddRangeAsync(fees,cancellationToken);
        }

        public Task ExecuteDeleteRanges(DeliveryFeeId[] feeIds, CancellationToken cancellationToken = default)
        {
            return _set.Where(fee => feeIds.Contains(fee.Id)).ExecuteDeleteAsync(cancellationToken);
        }
    }
}
