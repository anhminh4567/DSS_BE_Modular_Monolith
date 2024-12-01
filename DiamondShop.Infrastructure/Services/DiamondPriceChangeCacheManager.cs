using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.DiamondPrices;
using DiamondShop.Application.Usecases.DiamondCriterias.Commands.CreateFromRange;
using DiamondShop.Application.Usecases.DiamondCriterias.Commands.DeleteRange;
using DiamondShop.Application.Usecases.DiamondCriterias.Commands.UpdateRange;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Repositories;
using FluentEmail.Core.Interfaces;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services
{
    internal class DiamondPriceChangeCacheManager : IDiamondPriceChangeCacheManager
    {
        private readonly IDbCachingService _dbCachingService;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IUnitOfWork _unitOfWork;   
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private const string CriteriaCacheKey = "Criteria";
        private const string PriceCacheKey = "Price";
        public DiamondPriceChangeCacheManager(IDbCachingService dbCachingService, IDiamondPriceRepository diamondPriceRepository, IDiamondCriteriaRepository diamondCriteriaRepository, IUnitOfWork unitOfWork, IMapper mapper, ISender sender)
        {
            _dbCachingService = dbCachingService;
            _diamondPriceRepository = diamondPriceRepository;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sender = sender;
        }

        public List<DiamondCriteria> AddCriteria(List<CreateCriteriaFromRangeCommand> updateCaches)
        {
            
            throw new NotImplementedException();
        }

        public List<DiamondCriteria> DeleteCriteria(List<DeleteCriteriaByRangeCommand> updateCaches)
        {
            //var key = $"{CriteriaCacheKey}_Delete_{GenerateRandomness()}";
            //Dictionary<(float CaratFrom, float CaratTo), List<DiamondCriteria>> grouped = new();
            
            //if(updateCaches.)
            //var getCriteria = _diamondCriteriaRepository.GroupAllAvailableCaratRange();
            throw new NotImplementedException();
        }

        public List<DiamondPrice> SetPriceToCreate(List<DiamondPriceCreateCacheDto> updateCaches)
        {
            throw new NotImplementedException();
        }

        public List<DiamondPrice> SetPriceToRemove(List<DiamondPriceDeleteCacheDto> updateCaches)
        {
            throw new NotImplementedException();
        }

        public List<DiamondPrice> SetPriceToUpdate(List<DiamondPriceUpdateCacheDto> updateCaches)
        {
            throw new NotImplementedException();
        }

        public List<DiamondCriteria> UpdateCriteria(List<UpdateDiamondCriteriaRangeCommand> updateCaches)
        {
            throw new NotImplementedException();
        }
        private string GenerateRandomness()
        {
            return Utilities.GenerateRandomString(6);
        }
    }
}
