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
            throw new NotImplementedException();
        }
        private async Task ExpireDiamond(IJobExecutionContext context)
        {
            var expiredLockDiamonds = await _diamondRepository.GetLockDiamonds();
        }
    }
}
