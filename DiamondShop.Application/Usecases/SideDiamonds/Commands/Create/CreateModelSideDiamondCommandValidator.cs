﻿using DiamondShop.Application.Dtos.Requests.JewelryModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.SideDiamonds.Commands.Create
{
    public class CreateModelSideDiamondCommandValidator : AbstractValidator<CreateModelSideDiamondCommand>
    {
        public CreateModelSideDiamondCommandValidator()
        {
            RuleFor(c => c.ModelId)
                .NotEmpty();

            RuleFor(c => c.SideDiamondSpec)
                .NotEmpty()
                .ChildRules(p =>
                {
                    p.RuleFor(p => p.ColorMin)
                        .IsInEnum();

                    p.RuleFor(p => p.ColorMax)
                        .IsInEnum()
                        .Must((spec, max) => (int)spec.ColorMin <= (int)max).WithMessage((e) => $"Màu nhỏ nhất (\"{e.ColorMin}\") phải bé hơn màu lớn nhất (\"{e.ColorMax}\")");

                    p.RuleFor(p => p.ClarityMin)
                        .IsInEnum();

                    p.RuleFor(p => p.ClarityMax)
                        .IsInEnum()
                        .Must((spec, max) => (int)spec.ClarityMin <= (int)max).WithMessage((e) => $"Độ trong nhỏ nhất (\"{e.ClarityMin}\") phải bé hơn độ trong lớn nhất (\"{e.ClarityMax}\")");

                    p.RuleFor(p => p.SettingType)
                        .IsInEnum();

                    p.RuleFor(p => p.ShapeId)
                        .NotEmpty();

                    p.RuleFor(p => p.CaratWeight)
                                .NotNull()
                                .GreaterThan(0);

                    p.RuleFor(p => p.Quantity)
                        .NotNull()
                        .GreaterThan(0);

                });
        }
    }
}
