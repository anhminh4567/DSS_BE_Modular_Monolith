using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ErrorMessages;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryModelCategories.Commands.Delete
{
    public record DeleteJewelryCategoryCommand(string CategoryId) : IRequest<Result>;
    internal class DeleteJewelryCategoryCommandHandler : IRequestHandler<DeleteJewelryCategoryCommand, Result>
    {
        private readonly IJewelryModelCategoryRepository _categoryRepository;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        public DeleteJewelryCategoryCommandHandler(IJewelryModelCategoryRepository jewelryModelCategoryRepository, IUnitOfWork unitOfWork, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IJewelryModelRepository jewelryModelRepository)
        {
            _categoryRepository = jewelryModelCategoryRepository;
            _unitOfWork = unitOfWork;
            _optionsMonitor = optionsMonitor;
            _jewelryModelRepository = jewelryModelRepository;
        }

        public async Task<Result> Handle(DeleteJewelryCategoryCommand request, CancellationToken token)
        {
            request.Deconstruct(out string categoryId);
            await _unitOfWork.BeginTransactionAsync(token);
            var category = await _categoryRepository.GetById(JewelryModelCategoryId.Parse(categoryId));
            if (category == null)
                return JewelryModelErrors.Category.JewelryModelCategoryNotFoundError;
            var rules = _optionsMonitor.CurrentValue.JewelryModelCategoryRules;
            if (rules.DefaultCategories.Contains(category.Name))
                return JewelryModelErrors.Category.DeleteDefaultJewelryModelCategoryError;
            var exists = _jewelryModelRepository.ExistingCategory(category.Id);
            if (exists)
                return JewelryModelErrors.Category.DeleteJewelryModelCategoryInUseError;
            await _categoryRepository.Delete(category);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return Result.Ok();
        }
    }
}
