using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Diamonds.Queries.GetPaging;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Commands.UpdateBulkDiamondUpdatedDate
{
    public record UpdateDiamondUpdatedDateCommand(GetDiamond_4C diamond4c, string[] shapeIds, bool isLab ) : IRequest<Result>;
    internal class UpdateDiamondUpdatedDateCommandHandler : IRequestHandler<UpdateDiamondUpdatedDateCommand, Result>
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public UpdateDiamondUpdatedDateCommandHandler(IDiamondRepository diamondRepository, IUnitOfWork unitOfWork, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _diamondRepository = diamondRepository;
            _unitOfWork = unitOfWork;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result> Handle(UpdateDiamondUpdatedDateCommand request, CancellationToken cancellationToken)
        {
            var parsesShapeId = request.shapeIds.Select(x => DiamondShapeId.Parse(x)).ToList();
            var query = _diamondRepository.GetQuery();
            query = _diamondRepository.Filtering4C(query, request.diamond4c);
            query = _diamondRepository.QueryFilter(query, x => parsesShapeId.Contains(x.DiamondShapeId));
            query = _diamondRepository.QueryFilter(query, x => x.IsLabDiamond == request.isLab);
            await _diamondRepository.ExecuteUpdateDiamondUpdatedTime(query);
            return Result.Ok();
        }
    }
    
}
