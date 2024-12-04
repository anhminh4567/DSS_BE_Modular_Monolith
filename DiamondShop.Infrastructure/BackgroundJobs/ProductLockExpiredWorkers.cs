using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Services.interfaces;
using DiamondShop.Infrastructure.Databases;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium.DevTools.V127.Debugger;
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
        private readonly DiamondShopDbContext _dbContext;

        public ProductLockExpiredWorkers(IUnitOfWork unitOfWork, IDiamondRepository diamondRepository, IJewelryRepository jewelryRepository, IDiamondServices diamondServices, IJewelryService jewelryService, DiamondShopDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _diamondRepository = diamondRepository;
            _jewelryRepository = jewelryRepository;
            _diamondServices = diamondServices;
            _jewelryService = jewelryService;
            _dbContext = dbContext;
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
            if(expiredLockJewelry.Count <= 0)
                return;
            
            var jewelryIds = expiredLockJewelry.Select(x => x.Id).ToList();
            var jewelryDiamonds = await _dbContext.Diamonds.Where(x => x.JewelryId != null && jewelryIds.Contains(x.JewelryId)).ToListAsync();
            var timeNow = DateTime.UtcNow;
            await _unitOfWork.BeginTransactionAsync();
            foreach (var jewelry in expiredLockJewelry)
            {
                if (jewelry.ProductLock?.LockEndDate <= timeNow)
                {
                    var diamonds  = jewelryDiamonds.Where(jd => jd.JewelryId == jewelry.Id).ToList(); 
                    foreach (var diamond in diamonds)
                    {
                        diamond.RemoveLock();
                        _diamondRepository.Update(diamond).Wait();
                    }
                    jewelry.RemoveLock();
                    _jewelryRepository.Update(jewelry).Wait();
                }
            }
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
        }
    }
}
