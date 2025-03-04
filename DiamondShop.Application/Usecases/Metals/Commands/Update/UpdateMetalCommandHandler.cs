﻿using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
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

namespace DiamondShop.Application.Usecases.Metals.Commands.Update
{
    public record UpdateMetalCommand(string id, decimal price) : IRequest<Result<Metal>>;
    internal class UpdateMetalCommandHandler : IRequestHandler<UpdateMetalCommand, Result<Metal>>
    {
        private readonly IMetalRepository _metalRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateMetalCommandHandler(IMetalRepository metalRepository, IUnitOfWork unitOfWork)
        {
            _metalRepository = metalRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Metal>> Handle(UpdateMetalCommand request, CancellationToken cancellationToken)
        {
            MetalId metalId = MetalId.Parse(request.id);
            Metal getMetal = await _metalRepository.GetById(metalId);
            if (getMetal == null)
                return Result.Fail(new NotFoundError("Không tìm thấy kim loại"));
            getMetal.ChangePrice(request.price);
            await _metalRepository.Update(getMetal);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            return getMetal;
        }
    }
}
