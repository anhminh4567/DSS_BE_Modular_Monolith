using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Diamonds;
using DiamondShop.Application.Services.Interfaces.JewelryModels;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryModels.Files.Commands.RemoveThumbnail
{
    public record RemoveModelThumbnailCommand(string jewelryModelId) : IRequest<Result>;
    internal class RemoveModelThumbnailCommandHandler : IRequestHandler<RemoveModelThumbnailCommand, Result>
    {
        private readonly IJewelryModelFileService _jewelryModelFileService;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveModelThumbnailCommandHandler(IJewelryModelFileService jewelryModelFileService, IJewelryModelRepository jewelryModelRepository, IUnitOfWork unitOfWork)
        {
            _jewelryModelFileService = jewelryModelFileService;
            _jewelryModelRepository = jewelryModelRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemoveModelThumbnailCommand request, CancellationToken cancellationToken)
        {
            var id = JewelryModelId.Parse(request.jewelryModelId);
            var getJewelry = await _jewelryModelRepository.GetById(id);
            if (getJewelry is null)
                return Result.Fail(new NotFoundError("Diamond not found"));
            
            if (getJewelry.Thumbnail is null)
                return Result.Fail(new ConflictError("Diamond does not have thumbnail to delete, no need"));
            
            var thumbnailPath = getJewelry.Thumbnail.MediaPath;
            getJewelry.ChangeThumbnail(null);
            await _jewelryModelRepository.Update(getJewelry);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _jewelryModelFileService.DeleteFileAsync(thumbnailPath, cancellationToken);
            return Result.Ok();
        }
    }
}
