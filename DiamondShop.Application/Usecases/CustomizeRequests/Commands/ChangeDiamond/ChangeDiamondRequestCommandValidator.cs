using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.ChangeDiamond
{
    public class ChangeDiamondRequestCommandValidator : AbstractValidator<ChangeDiamondRequestCommand>
    {
        public ChangeDiamondRequestCommandValidator()
        {
            RuleFor(p => p.CustomizeRequestId).NotEmpty();
            RuleFor(p => p.DiamondRequestId).NotEmpty();
            RuleFor(p => p.DiamondId).NotEmpty().When(p => p.CreateDiamondCommand == null).WithMessage("Id kim cương đang trống");
            RuleFor(p => p.CreateDiamondCommand).NotEmpty().When(p => p.DiamondId == null).WithMessage("Yêu cầu kim cương mới đang trống");
        }
    }
}
