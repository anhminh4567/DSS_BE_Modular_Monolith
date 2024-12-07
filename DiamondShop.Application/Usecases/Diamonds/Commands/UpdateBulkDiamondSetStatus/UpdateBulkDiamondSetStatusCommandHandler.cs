using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Commands.UpdateBulkDiamondSetStatus
{
    public record UpdateBulkDiamondSetStatusCommand(GetDiamond_4C diamond4c, string[] shapeIds, bool isLab, bool isSetInactive = true) : IRequest<Result>;
    internal class UpdateBulkDiamondSetStatusCommandHandler : IRequestHandler<UpdateBulkDiamondSetStatusCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public UpdateBulkDiamondSetStatusCommandHandler(IUnitOfWork unitOfWork, IDiamondRepository diamondRepository, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _unitOfWork = unitOfWork;
            _diamondRepository = diamondRepository;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result> Handle(UpdateBulkDiamondSetStatusCommand request, CancellationToken cancellationToken)
        {
            var parsesShapeId = request.shapeIds.Select(x => DiamondShapeId.Parse(x)).ToList();
            var query = _diamondRepository.GetQuery();
            query = _diamondRepository.Filtering4C(query, request.diamond4c);
            query = _diamondRepository.QueryFilter(query, x => parsesShapeId.Contains(x.DiamondShapeId));
            query = _diamondRepository.QueryFilter(query, x => x.IsLabDiamond == request.isLab);
            var result = query.ToList();
            foreach (var diamond in result)
            {
                if (request.isSetInactive)
                {
                    diamond.SetActive(false);
                }
                else
                {
                    diamond.SetActive(true);
                }
                _diamondRepository.Update(diamond).Wait();
            }
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok();
        }
    }
    
}
