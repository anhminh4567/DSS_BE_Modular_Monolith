﻿using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Domain.Models.Promotions.Enum;
using FluentValidation;

namespace DiamondShop.Application.Usecases.PromotionRequirements.Commands.CreateMany
{
    public class CreateRequirementCommandValidator : AbstractValidator<CreateRequirementCommand>
    {
        public CreateRequirementCommandValidator()
        {
            var validator = new RequirementSpecValidator();
            RuleFor(x => x.Requirements).Cascade(CascadeMode.Stop).NotNull();
            RuleForEach(x => x.Requirements).SetValidator(validator);
        }
    }
    public class RequirementSpecValidator : AbstractValidator<RequirementSpec>
    {
        public RequirementSpecValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(2);
            RuleFor(x => x.TargetType).IsInEnum().WithMessage("TargetType is not valid.");
            RuleFor(x => x.Operator).IsInEnum().WithMessage("Operator is not valid.");
            RuleFor(x => x.MoneyAmount).GreaterThan(1000)
                .When(x => x.MoneyAmount.HasValue && x.isPromotion == true)
                .WithMessage("MoneyAmount should be greater than 1000.")
                .WithName((spec ) => "requirement named "+ spec.Name);
            When(x => x.TargetType == TargetType.Diamond, () =>
            {
                ClassLevelCascadeMode = CascadeMode.Stop;
                RuleFor(x => x.DiamondRequirementSpec).NotNull().WithMessage("diamond requirement specification is null for target diamond")
                .WithName((spec) => "DiamondRequirementSpec, named: "+ spec.Name);

                RuleFor(x => x.DiamondRequirementSpec.Origin)
                .IsInEnum()
                .WithMessage("not valid origin, can only be lab = 1, natural = 2, both = 3 ");

                RuleFor(x => x.DiamondRequirementSpec.caratFrom).GreaterThanOrEqualTo(0).WithMessage("carat must greater than 0");
                RuleFor(x => x.DiamondRequirementSpec.caratTo).GreaterThanOrEqualTo(0).WithMessage("carat must greater than 0");
                RuleFor(x => x.DiamondRequirementSpec.caratFrom).LessThanOrEqualTo(x => x.DiamondRequirementSpec.caratTo).WithMessage("carat from must be less than carat to"); ;
                RuleFor(x => x.DiamondRequirementSpec.cutFrom)
                .IsInEnum()
                .WithIsInEnumMessage();
                RuleFor(x => x.DiamondRequirementSpec.cutTo)
                .IsInEnum()
                .WithIsInEnumMessage();
                RuleFor(x => x.DiamondRequirementSpec.colorFrom)
                .IsInEnum()
                .WithIsInEnumMessage();
                RuleFor(x => x.DiamondRequirementSpec.colorTo)
                .IsInEnum()
                .WithIsInEnumMessage();
                RuleFor(x => x.DiamondRequirementSpec.clarityFrom)
                .IsInEnum()
                .WithIsInEnumMessage();
                RuleFor(x => x.DiamondRequirementSpec.clarityTo)
                .IsInEnum()
                .WithIsInEnumMessage();

                RuleFor(x => (int)x.DiamondRequirementSpec.cutFrom).LessThanOrEqualTo(x => (int)x.DiamondRequirementSpec.cutTo).WithMessage("cut from must be less than cut to"); 
                RuleFor(x => (int)x.DiamondRequirementSpec.clarityFrom).LessThanOrEqualTo(x => (int)x.DiamondRequirementSpec.clarityTo).WithMessage("clarity from must be less than clarity to");
                RuleFor(x => (int)x.DiamondRequirementSpec.colorFrom).LessThanOrEqualTo(x => (int)x.DiamondRequirementSpec.colorTo).WithMessage("color from must be less than color to");
            });
            When(x => x.TargetType == TargetType.Jewelry_Model, () =>
            {
                RuleFor(x => x.JewelryModelID).NotEmpty();
            });
        }
    }
}
