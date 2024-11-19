using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.Proceed.Staff
{
    public class StaffProceedCustomizeRequestCommandValidator : AbstractValidator<StaffProceedCustomizeRequestCommand>
    {
        public StaffProceedCustomizeRequestCommandValidator()
        {
            RuleFor(p => p.RequestId).NotEmpty();
            RuleForEach(p => p.DiamondAssigning).ChildRules(p =>
            {
                p.RuleFor(k => k.DiamondRequestId).NotEmpty();
                p.RuleFor(k => k.DiamondId).NotEmpty().When(k => k.CreateDiamondCommand == null).WithMessage("Kim cương đang trống");
                p.RuleFor(k => k.CreateDiamondCommand).NotEmpty().When(k => k.DiamondId == null).WithMessage("Yêu cầu kim cương mới đang trống");
            }).When(p => p.DiamondAssigning != null);
        }
    }
}
