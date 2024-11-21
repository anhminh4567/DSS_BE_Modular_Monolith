using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryModels.Commands.Delete
{
    public class DeleteJewelryModelCommandValidator : AbstractValidator<DeleteJewelryModelCommand>
    {
        public DeleteJewelryModelCommandValidator()
        {
            RuleFor(p => p.ModelId).NotEmpty();
        }
    }
}
