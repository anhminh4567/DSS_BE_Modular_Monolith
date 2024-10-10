using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using FluentValidation;

namespace DiamondShop.Application.Usecases.JewelrySideDiamonds.Create
{
    public class CreateJewelrySideDiamondCommandValidator : AbstractValidator<CreateJewelrySideDiamondCommand>
    {
        public CreateJewelrySideDiamondCommandValidator()
        {
            RuleFor(p => p.JewelryId)
                .NotEmpty();

            RuleFor(p => p.SideDiamondOptIds)
                .NotEmpty()
                .Must(CheckUnique).WithMessage("Jewelry side diamonds can't contain duplicated option");   
        
        
        }
        private bool CheckUnique(List<SideDiamondOptId> ids)
        {
            return ids.Count() == ids.Distinct().Count();
        }
    }
}
