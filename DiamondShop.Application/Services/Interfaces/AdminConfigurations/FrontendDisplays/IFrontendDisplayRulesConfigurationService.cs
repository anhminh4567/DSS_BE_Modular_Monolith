using DiamondShop.Application.Commons.Models;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces.AdminConfigurations.FrontendDisplays
{
    public interface IFrontendDisplayRulesConfigurationService : IBaseConfigurationService<FrontendDisplayConfiguration>
    {
        Task<List<Media>> GetAllCarouselImages();
        Task<Media> SetNewImage(FileData newImage);
        Task RemoveImage(string absolutePath);
    }
}
