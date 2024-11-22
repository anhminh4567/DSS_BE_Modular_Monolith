using DiamondShop.Domain.Models.CustomizeRequests.Entities;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.interfaces
{
    public interface IMainDiamondService
    {
        public Task<Result> CheckMatchingDiamond(JewelryModelId jewelryModelId, List<Diamond> diamonds);
        public Task<Result> CheckMatchingDiamond(JewelryModelId jewelryModelId, List<DiamondRequest> customizeRequests);
    }
}
