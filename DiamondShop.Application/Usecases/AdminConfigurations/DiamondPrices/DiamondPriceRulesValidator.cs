using DiamondShop.Domain.BusinessRules;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.AdminConfigurations.DiamondPrices
{
    internal class DiamondPriceRulesValidator : AbstractValidator<DiamondPriceRules>
    {
        public DiamondPriceRulesValidator()
        {
            RuleFor(x => x.DefaultFancyCriteriaPriceBoard).NotEmpty();
            RuleFor(x => x.DefaultRoundCriteriaPriceBoard).NotEmpty();
            RuleFor(x => x.DefaultSideDiamondCriteriaPriceBoard).NotEmpty();
        }
    }
}
