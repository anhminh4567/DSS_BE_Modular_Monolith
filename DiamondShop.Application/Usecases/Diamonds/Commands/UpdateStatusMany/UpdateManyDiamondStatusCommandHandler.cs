using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Commands.UpdateStatusMany
{
    public record UpdateDiamondStateRequest(string diamondId, bool setActive); 
    public record UpdateManyDiamondStatusCommand(List<UpdateDiamondStateRequest> UpdateDiamondStates) : IRequest<Result>;
    internal class UpdateManyDiamondStatusCommandHandler : IRequestHandler<UpdateManyDiamondStatusCommand, Result>
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateManyDiamondStatusCommandHandler(IDiamondRepository diamondRepository, IUnitOfWork unitOfWork)
        {
            _diamondRepository = diamondRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateManyDiamondStatusCommand request, CancellationToken cancellationToken)
        {
            var mappedDiamonds = request.UpdateDiamondStates.Select(x => new { Id = DiamondId.Parse(x.diamondId), IsActive = x.setActive  }).ToList();
            var diamonds = await _diamondRepository.GetRange(mappedDiamonds.Select(x => x.Id).ToList());
            foreach (var diamond in diamonds)
            {
                var status = mappedDiamonds.FirstOrDefault(x => x.Id == diamond.Id);
                if(status != null)
                {
                    diamond.SetActive(status.IsActive);
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
