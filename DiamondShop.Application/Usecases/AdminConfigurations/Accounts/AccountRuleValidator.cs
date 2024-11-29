using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Domain.BusinessRules;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.AdminConfigurations.Accounts
{
    public class AccountRuleValidator : AbstractValidator<AccountRules>
    {
        public AccountRuleValidator()
        {
            RuleFor(x => x.VndPerPoint)
                .GreaterThan(1000).WithGreaterThanMessage()
                .LessThan(100_000_000).WithLessThanMessage();
                
            RuleFor(x => x.TotalPointToBronze)
                .GreaterThan(0).WithGreaterThanMessage()
                .LessThan((x ) => x.TotalPointToSilver).WithLessThanMessage();
            RuleFor(x => x.TotalPointToSilver)
                .GreaterThan(0).WithGreaterThanMessage()
                .LessThan((x) => x.TotalPointToGold).WithLessThanMessage();
            RuleFor(x => x.TotalPointToGold)
                .GreaterThan(0).WithGreaterThanMessage();
        }
    }
}
