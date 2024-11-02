using FluentValidation;

namespace DiamondShop.Application.Usecases.DiamondCriterias.Commands.UpdateRange
{
    public class UpdateDiamondCriteriaRangeCommandValidator : AbstractValidator<UpdateDiamondCriteriaRangeCommand>
    {
        public UpdateDiamondCriteriaRangeCommandValidator()
        {
            RuleFor(x => x.oldCaratRange).Cascade(CascadeMode.Stop).NotNull();
            RuleFor(x => x.newCaratRange).Cascade(CascadeMode.Stop).NotNull();
            When(x => x.oldCaratRange != null && x.newCaratRange != null, () =>
            {
                RuleFor(x => x.oldCaratRange.caratFrom).GreaterThanOrEqualTo(0);
                RuleFor(x => x.oldCaratRange.caratFrom).GreaterThanOrEqualTo(0);
                RuleFor(x => x.oldCaratRange.caratFrom).LessThan(x => x.oldCaratRange.caratTo);
                RuleFor(x => x.newCaratRange.caratFrom).LessThan(x => x.newCaratRange.caratTo);
            });
        }
    }
}
