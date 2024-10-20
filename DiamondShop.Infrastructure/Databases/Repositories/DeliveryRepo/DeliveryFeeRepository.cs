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

        public Task<List<DeliveryFee>> GetLocationType(CancellationToken cancellationToken = default)
        {
            return _set.Where(d => d.FromLocation != null && d.ToLocation != null).ToListAsync(cancellationToken);
        }

        public Task<DeliveryFee?> GetWithDistance(decimal distant, CancellationToken cancellationToken = default)
        {
            return _set.Where(d => d.FromKm <= distant && d.ToKm >= distant).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
