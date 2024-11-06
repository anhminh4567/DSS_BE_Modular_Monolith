using DiamondShop.Api.Controllers.JewelryModels.UpdateFee;
using FluentValidation;

namespace DiamondShop.Application.Usecases.JewelryModels.Commands.UpdateFee
{
    public class UpdateModelFeeCommandValidator : AbstractValidator<UpdateModelFeeCommand>
    {
        public UpdateModelFeeCommandValidator()
        {
            RuleFor(p => p.ModelId).NotEmpty();
            RuleFor(p => p.NewFee).GreaterThan(0);
        }
    }
}
