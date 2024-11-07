using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondPrices.Commands.ChangeCutOffset
{
    public record ChangeCutOffsetRequetsDto (decimal oldOffset, decimal newOffset);
    public record UpdateDiamondPriceCutOffsetPercentCommand(bool isFancyShape , ChangeCutOffsetRequetsDto changeCutOffset) : IRequest<Result<DiamondRule>>;
    internal class UpdateDiamondPriceCutOffsetPercentCommandHandler : IRequestHandler<UpdateDiamondPriceCutOffsetPercentCommand, Result<DiamondRule>>
    {
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDiamondPriceCutOffsetPercentCommandHandler(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IUnitOfWork unitOfWork)
        {
            _optionsMonitor = optionsMonitor;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<DiamondRule>> Handle(UpdateDiamondPriceCutOffsetPercentCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
