using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Jewelries.Queries.GetAvailable
{
    public class GetAvailableJewelryQueryValidator : AbstractValidator<GetAvailableJewelryQuery>
    {
        public GetAvailableJewelryQueryValidator()
        {
            RuleFor(p => p.ModelId).NotEmpty();
            RuleFor(p => p.MetalId).NotEmpty();
            RuleFor(p => p.SizeId).NotEmpty();
        }
    }
}
