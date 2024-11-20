using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Diamonds;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Diamonds.ErrorMessages;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Files.Commands.RemoveThumbnail
{
    public  record RemoveThumbnailCommand(string diamond) : IRequest<Result>;
    internal class RemoveThumbnailCommandHandler : IRequestHandler<RemoveThumbnailCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondFileService _diamondFileService;
        public RemoveThumbnailCommandHandler(IUnitOfWork unitOfWork, IDiamondRepository diamondRepository, IDiamondFileService diamondFileService)
        {
            _unitOfWork = unitOfWork;
            _diamondRepository = diamondRepository;
            _diamondFileService = diamondFileService ;
        }

        public async Task<Result> Handle(RemoveThumbnailCommand request, CancellationToken cancellationToken)
        {
            var id = DiamondId.Parse(request.diamond);
            var getDiamond = await _diamondRepository.GetById(id);
            if(getDiamond is null )
            {
                return Result.Fail(DiamondErrors.DiamondNotFoundError);
            }
            if(getDiamond.Thumbnail is null)
            {
                return Result.Fail(new ConflictError("không có thumbnail để delete , legit"));
            }
            var thumbnailPath = getDiamond.Thumbnail.MediaPath;
            getDiamond.ChangeThumbnail(null);
            await _diamondRepository.Update(getDiamond);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _diamondFileService.DeleteFileAsync(thumbnailPath,cancellationToken);
            return Result.Ok();
        }
    }
}
