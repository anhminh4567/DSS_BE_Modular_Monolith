using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Domain.BusinessRules;
using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.AdminConfigurations.BankAccounts
{
    public class ShopBankAccountRulesValidator : AbstractValidator<ShopBankAccountRules>
    {
        public ShopBankAccountRulesValidator()
        {
            RuleFor(x => x.AccountNumber).NotEmpty().WithNotEmptyMessage();
            RuleFor(x => x.AccountName).NotEmpty().WithNotEmptyMessage();
            RuleFor(x => x.BankBin).NotEmpty().WithNotEmptyMessage();
        }
    }
}
