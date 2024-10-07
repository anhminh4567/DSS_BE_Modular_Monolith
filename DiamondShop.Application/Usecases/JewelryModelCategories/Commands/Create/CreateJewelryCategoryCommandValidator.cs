using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryModelCategories.Commands.Create
{
    public class CreateJewelryCategoryCommandValidator : AbstractValidator<CreateJewelryCategoryCommand>
    {
        public CreateJewelryCategoryCommandValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
