using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Warranties;
using DiamondShop.Domain.Models.Warranties.Enum;
using DiamondShop.Domain.Models.Warranties.ErrorMessages;
using DiamondShop.Domain.Models.Warranties.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace DiamondShop.Api.Controllers.Warranties.Delete
{
    public record DeleteWarrantyCommand(string WarrantyId) : IRequest<Result<Warranty>>;
    internal class DeleteWarrantyCommandHandler : IRequestHandler<DeleteWarrantyCommand, Result<Warranty>>
    {
        private readonly IWarrantyRepository _warrantyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        public DeleteWarrantyCommandHandler(IWarrantyRepository warrantyRepository, IUnitOfWork unitOfWork, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _warrantyRepository = warrantyRepository;
            _unitOfWork = unitOfWork;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<Warranty>> Handle(DeleteWarrantyCommand request, CancellationToken token)
        {
            request.Deconstruct(out string? warrantyId);
            await _unitOfWork.BeginTransactionAsync(token);
            var warranty = await _warrantyRepository.GetById(WarrantyId.Parse(warrantyId));
            if (warranty == null)
                return Result.Fail(WarrantyErrors.WarrantyNotFoundError);
            if (_optionsMonitor.CurrentValue.WarrantyRules.DEFAULT_CODE.Contains(warranty.Code))
                return Result.Fail(WarrantyErrors.DeleteDefaultConflictError);
            await _warrantyRepository.Delete(warranty);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return warranty;
        }
    }
}
