using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Services.interfaces;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.BackgroundJobs
{
    internal class ProductLockExpiredWorkers : IJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IDiamondServices _diamondServices;
        private readonly IJewelryService _jewelryService;

        public ProductLockExpiredWorkers(IUnitOfWork unitOfWork, IDiamondRepository diamondRepository, IJewelryRepository jewelryRepository, IDiamondServices diamondServices, IJewelryService jewelryService)
        {
            _unitOfWork = unitOfWork;
            _diamondRepository = diamondRepository;
            _jewelryRepository = jewelryRepository;
            _diamondServices = diamondServices;
            _jewelryService = jewelryService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await ExpireDiamond(context);
            await ExpireJewelry(context);
        }
        private async Task ExpireDiamond(IJobExecutionContext context)
        {
            var expiredLockDiamonds = await _diamondRepository.GetLockDiamonds();
            var timeNow = DateTime.UtcNow;
            foreach(var expiredDiamond in expiredLockDiamonds)
            {
                if(expiredDiamond.ProductLock?.LockEndDate <= timeNow)
                {
                    expiredDiamond.RemoveLock();
                    _diamondRepository.Update(expiredDiamond).Wait();
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }
        private async Task ExpireJewelry(IJobExecutionContext context)
        {
            var expiredLockJewelry = await _jewelryRepository.GetLockJewelry();
            var timeNow = DateTime.UtcNow;
            foreach (var jewelry in expiredLockJewelry)
            {
                if (jewelry.ProductLock?.LockEndDate <= timeNow)
                {
                    jewelry.RemoveLock();
                    _jewelryRepository.Update(jewelry).Wait();
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
