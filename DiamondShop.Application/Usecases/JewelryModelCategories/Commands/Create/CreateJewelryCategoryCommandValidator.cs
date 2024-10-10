using FluentValidation;

namespace DiamondShop.Application.Usecases.JewelryModelCategories.Commands.Create
{
    public class CreateJewelryCategoryCommandValidator : AbstractValidator<CreateJewelryCategoryCommand>
    {
        public CreateJewelryCategoryCommandValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty();

        }
    }
}
