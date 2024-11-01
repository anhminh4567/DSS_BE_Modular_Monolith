using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.SizeMetals.Commands.Delete
{
    public class DeleteModelSideDiamondCommandValidator : AbstractValidator<DeleteModelSideDiamondCommand>
    {
        public DeleteModelSideDiamondCommandValidator() { 
            RuleFor(p => p.SideDiamondOptId).NotEmpty();
        }
    }
}
