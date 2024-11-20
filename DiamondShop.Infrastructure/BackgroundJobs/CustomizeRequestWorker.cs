using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DiamondShop.Infrastructure.BackgroundJobs
{
    public class CustomizeRequestWorker : IJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CustomizeRequestWorker> _logger;
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IDiamondRepository _diamondRepository;

        public CustomizeRequestWorker(IUnitOfWork unitOfWork, ICustomizeRequestRepository customizeRequestRepository, IDiamondRepository diamondRepository, IJewelryRepository jewelryRepository, ILogger<CustomizeRequestWorker> logger)
        {
            _unitOfWork = unitOfWork;
            _customizeRequestRepository = customizeRequestRepository;
            _diamondRepository = diamondRepository;
            _jewelryRepository = jewelryRepository;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var query = _customizeRequestRepository.GetQuery();
            var timeNow = DateTime.UtcNow;
            query = query.Where(p =>
            p.Status != CustomizeRequestStatus.Shop_Rejected &&
            p.Status != CustomizeRequestStatus.Customer_Rejected &&
            p.ExpiredDate <= timeNow).Include(x => x.DiamondRequests);
            if (query.Count() > 0)
            {
                var list = query.ToList();
                foreach (var p in list)
                {
                    if (p.Status == CustomizeRequestStatus.Accepted)
                    {
                        p.SetCustomerCancel();
                        await HandleJewelryCancelled(context, p);
                        await HandleDiamondRequestCancelled(context, p);
                    }
                    //Request reaches expired date
                    else
                    {
                        if (p.Status == CustomizeRequestStatus.Priced)
                            await HandleDiamondRequestCancelled(context, p);
                        p.SetShopReject();
                    }
                }
                _customizeRequestRepository.UpdateRange(list);
                await _unitOfWork.SaveChangesAsync();
            }
        }
        private async Task HandleDiamondRequestCancelled(IJobExecutionContext context, CustomizeRequest requestDetail)
        {
            if (requestDetail.DiamondRequests.Count > 0)
            {
                var diamondIds = requestDetail.DiamondRequests.Select(x => x.DiamondId).ToList();
                var diamonds = await _diamondRepository
                    .GetQuery()
                    .Where(x => diamondIds.Contains(x.Id)).ToListAsync();
                var preOrderDiamonds = diamonds.Where(x => x.Status == Domain.Common.Enums.ProductStatus.PreOrder).ToList();
                preOrderDiamonds.ForEach(x => _diamondRepository.Delete(x));
                await _unitOfWork.SaveChangesAsync();
            }
        }
        private async Task HandleJewelryCancelled(IJobExecutionContext context, CustomizeRequest requestDetail)
        {
            if (requestDetail.JewelryId == null)
                _logger.LogError($"Can't get jewelry id from request {requestDetail.Id.Value}");
            else
            {
                var jewelry = await _jewelryRepository.GetById(requestDetail.JewelryId);
                if (jewelry == null)
                    _logger.LogError($"Can't get jewelry with id {requestDetail.JewelryId.Value}");
                await _jewelryRepository.Delete(jewelry);
            }
        }

    }
}
