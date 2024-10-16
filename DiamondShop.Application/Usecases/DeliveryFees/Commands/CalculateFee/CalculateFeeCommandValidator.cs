using FluentValidation;

namespace DiamondShop.Application.Usecases.DeliveryFees.Commands.CalculateFee
{
    public class CalculateFeeCommandValidator : AbstractValidator<CalculateFeeCommand>
    {
        public CalculateFeeCommandValidator()
        {
            RuleFor(x => x.Province).NotEmpty().WithMessage("Province is required");   
            RuleFor(x => x.District).NotEmpty().WithMessage("District is required");
            RuleFor(x => x.Ward).NotEmpty().WithMessage("Ward is required");
            RuleFor(x => x.Street).NotEmpty().WithMessage("Street is required");
        }
    }

}
