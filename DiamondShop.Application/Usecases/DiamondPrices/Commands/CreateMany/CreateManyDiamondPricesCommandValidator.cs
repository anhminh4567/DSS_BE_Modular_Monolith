﻿using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Application.Dtos.Requests.Diamonds;
using FluentValidation;

namespace DiamondShop.Application.Usecases.DiamondPrices.Commands.CreateMany
{
    public class CreateManyDiamondPricesCommandValidator : AbstractValidator<CreateManyDiamondPricesCommand>
    {
        public CreateManyDiamondPricesCommandValidator()
        {
            RuleFor(x => x.listPrices).NotNull().NotEmpty().WithNotEmptyMessage();
            RuleForEach(x => x.listPrices).SetValidator(new DiamondPriceRequestDtoValidator());
        }
        private class DiamondPriceRequestDtoValidator : AbstractValidator<DiamondPriceRequestDto>
        {
            public DiamondPriceRequestDtoValidator()
            {
                RuleFor(c => c.price).NotEmpty().GreaterThan(0);
                When(x => x.Color != null, () =>
                {
                    RuleFor(c => c.Color).IsInEnum().WithIsInEnumMessage();
                });
                When(x => x.Clarity != null, () =>
                {
                    RuleFor(c => c.Clarity).IsInEnum().WithIsInEnumMessage();
                });
                //When(x => x.cut != null, () =>
                //{
                //    RuleFor(c => c.cut).IsInEnum().WithIsInEnumMessage();
                //});

            }
        }
    }
}
