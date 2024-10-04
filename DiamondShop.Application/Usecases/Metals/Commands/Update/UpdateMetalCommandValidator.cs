using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Metals.Commands.Update
{
    public class UpdateMetalCommandValidator : AbstractValidator<UpdateMetalCommand>
    {
        public UpdateMetalCommandValidator() 
        { 
            RuleFor(c => c.price)
                .NotNull().GreaterThanOrEqualTo(0);
        }
    }
}
