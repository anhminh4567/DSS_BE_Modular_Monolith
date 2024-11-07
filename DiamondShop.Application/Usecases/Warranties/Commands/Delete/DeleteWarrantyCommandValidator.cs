using DiamondShop.Api.Controllers.Warranties.Delete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Warranties.Commands.Delete
{
    public class DeleteWarrantyCommandValidator : AbstractValidator<DeleteWarrantyCommand>
    {
        public DeleteWarrantyCommandValidator()
        {
            RuleFor(p => p.WarrantyId).NotEmpty();
        }
    }
}
