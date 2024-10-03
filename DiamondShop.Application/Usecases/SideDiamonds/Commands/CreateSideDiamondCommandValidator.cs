using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.SideDiamonds.Commands
{
    public class CreateSideDiamondCommandValidator : AbstractValidator<CreateSideDiamondCommand>
    {
        public CreateSideDiamondCommandValidator()
        {
            
        }
    }
}
