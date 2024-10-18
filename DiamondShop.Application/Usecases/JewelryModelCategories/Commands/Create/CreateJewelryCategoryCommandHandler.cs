using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryModelCategories.Commands.Create
{
    public record CreateJewelryCategoryCommand(string Name, string Description, bool IsGeneral, string? ParentCategoryId) : IRequest<Result<JewelryModelCategory>>;
    internal class CreateJewelryCategoryCommandHandler : IRequestHandler<CreateJewelryCategoryCommand, Result<JewelryModelCategory>>
    {
        private readonly IJewelryModelCategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateJewelryCategoryCommandHandler(
            IJewelryModelCategoryRepository categoryRepository,
            IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<JewelryModelCategory>> Handle(CreateJewelryCategoryCommand request, CancellationToken token)
        {
            string capitalized = Utilities.Capitalize(request.Name);
            var flag1 = await _categoryRepository.CheckDuplicate(capitalized);
            if (flag1) return Result.Fail("This category name has already been used");
            var parentId = request.ParentCategoryId is null ? null : JewelryModelCategoryId.Parse(request.ParentCategoryId);
            var category = JewelryModelCategory.Create(capitalized, request.Description, "", request.IsGeneral, parentId);
            await _categoryRepository.Create(category, token);
            await _unitOfWork.SaveChangesAsync(token);
            return category;
        }
    }
}
