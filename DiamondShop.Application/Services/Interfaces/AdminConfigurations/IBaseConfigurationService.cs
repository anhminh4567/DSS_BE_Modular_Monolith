using DiamondShop.Application.Services.Interfaces.AdminConfigurations.FrontendDisplays;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces.AdminConfigurations
{
    public interface IBaseConfigurationService<TConfiguration> where TConfiguration : class
    {
        Task<TConfiguration> GetConfiguration();
        Task<Result> SetConfiguration(TConfiguration newValidatedConfiguration);
    }
}
