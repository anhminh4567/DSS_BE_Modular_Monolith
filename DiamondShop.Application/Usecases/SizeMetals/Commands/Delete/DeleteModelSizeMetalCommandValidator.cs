using DiamondShop.Application.Usecases.SizeMetals.Commands.Update;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.SizeMetals.Commands.Delete
{
    public class DeleteModelSizeMetalCommandValidator : AbstractValidator<DeleteModelSizeMetalCommand>
    {
        public DeleteModelSizeMetalCommandValidator()
        {
            RuleFor(p => p.ModelId).NotEmpty();
            RuleFor(p => p.MetalId).NotEmpty();
            RuleFor(p => p.SizeId).NotEmpty();
        }
    }
}
