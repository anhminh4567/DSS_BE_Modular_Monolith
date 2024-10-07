using DiamondShop.Application.Dtos.Requests.JewelryModels;
using FluentValidation;

namespace DiamondShop.Application.Usecases.SizeMetals.Commands.Create
{
    public class CreateSizeMetalCommandValidator : AbstractValidator<CreateSizeMetalCommand>
    {
        public CreateSizeMetalCommandValidator()
        {
            RuleFor(c => c.ModelId)
                .NotEmpty();
            RuleFor(c => c.MetalSizeSpecs)
                .NotEmpty()
                .Must(CheckDuplicateItem).WithMessage("No duplicates allowed");
            RuleForEach(c => c.MetalSizeSpecs)
                .NotEmpty()
                .ChildRules(p =>
                {
                    p.RuleFor(p => p.MetalId)
                        .NotEmpty();

                    p.RuleFor(p => p.SizeId)
                        .NotEmpty();

                    p.RuleFor(p => p.Weight)
                        .NotNull()
                        .GreaterThan(0f);
                })
                ;
        }
        private bool CheckDuplicateItem(List<ModelMetalSizeRequestDto> spec)
        {
            Dictionary<(string metal,string size),bool> set = new();
            foreach(var item in spec)
            {
                (string metal, string size) holder = new(item.MetalId, item.SizeId);
                if (set.ContainsKey(holder)) return false;
                else set.Add(holder, true);
            }
            return true;
        }
    }
}
