using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryModels.Commands.Create
{
    public class CreateJewelryModelCommandValidator : AbstractValidator<CreateJewelryModelCommand>
    {
        public CreateJewelryModelCommandValidator()
        {
            RuleFor(c => c.ModelSpec).NotEmpty()
                .ChildRules(items =>
                {
                    items.RuleFor(c => c.Name).NotEmpty();
                });
        }
    }
}
