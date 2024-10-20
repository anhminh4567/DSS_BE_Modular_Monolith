using DiamondShop.Domain.Models.DeliveryFees;
using DiamondShop.Domain.Models.DeliveryFees.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.DeliveryRepo
{
    public interface IDeliveryFeeRepository : IBaseRepository<DeliveryFee>
    {
        Task CreateRange(List<DeliveryFee> fees, CancellationToken cancellationToken = default);
        Task ExecuteDeleteRanges(DeliveryFeeId[] feeIds, CancellationToken cancellationToken =default);
        Task<DeliveryFee?> GetWithDistance(decimal distant, CancellationToken cancellationToken = default);
        Task<List<DeliveryFee>> GetLocationType(CancellationToken cancellationToken = default);
    }
}
