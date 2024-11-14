using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.Reject.Staff
{
    public class StaffRejectRequestCommandValidator : AbstractValidator<StaffRejectRequestCommand>
    {
        public StaffRejectRequestCommandValidator()
        {
            RuleFor(p => p.CustomizeRequestId).NotEmpty();
        }
    }
}
