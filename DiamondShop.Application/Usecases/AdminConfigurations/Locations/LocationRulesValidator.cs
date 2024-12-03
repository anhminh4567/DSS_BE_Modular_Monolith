using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Domain.BusinessRules;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.AdminConfigurations.Locations
{
    public class LocationRulesValidator : AbstractValidator<LocationRules>
    {
        public LocationRulesValidator()
        {
            RuleFor(x => x.OrignalWard).NotEmpty()
                .WithNotEmptyMessage();
            RuleFor(x => x.OrignalRoad).NotEmpty()
                .WithNotEmptyMessage();
            RuleFor(x => x.OrignalDistrict).NotEmpty()
                            .WithNotEmptyMessage();
            RuleFor(x => x.OriginalLocationName).NotEmpty()
                            .WithNotEmptyMessage();
            RuleFor(x => x.OriginalProvince).NotEmpty()
                            .WithNotEmptyMessage();

        }
    }
}
