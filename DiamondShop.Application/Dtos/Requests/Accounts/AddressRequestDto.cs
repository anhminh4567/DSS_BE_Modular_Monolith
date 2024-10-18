using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Requests.Accounts
{
    public class AddressRequestDto
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Street { get; set; }

    }
    public class AddressRequestDtoValidator : AbstractValidator<AddressRequestDto>
    {
        public AddressRequestDtoValidator()
        {
            RuleFor(x => x.Street).NotEmpty().WithMessage("Street must not be empty");
            RuleFor(x => x.Province).NotEmpty().WithMessage("City must not be empty");
            RuleFor(x => x.District).NotEmpty().WithMessage("State must not be empty");
            RuleFor(x => x.Ward).NotEmpty().WithMessage("Zip code must not be empty");
        }
    }
}
