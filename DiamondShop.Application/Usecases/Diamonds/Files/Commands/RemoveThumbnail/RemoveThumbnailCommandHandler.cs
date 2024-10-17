using DiamondShop.Application.Services.Interfaces;
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
        private readonly IBlobFileServices _blobFileServices;

        public RemoveThumbnailCommandHandler(IUnitOfWork unitOfWork, IDiamondRepository diamondRepository, IBlobFileServices blobFileServices)
        {
            _unitOfWork = unitOfWork;
            _diamondRepository = diamondRepository;
            _blobFileServices = blobFileServices;
        }

        public async Task<Result> Handle(RemoveThumbnailCommand request, CancellationToken cancellationToken)
        {
            var id = DiamondId.Parse(request.diamond);
            var getDiamond = await _diamondRepository.GetById(id);
            if(getDiamond is null )
            {
                return Result.Fail("Diamond not found");
            }
            var thumbnailPath = getDiamond.Thumbnail.MediaPath;
            getDiamond.ChangeThumbnail(null);
            await _diamondRepository.Update(getDiamond);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _blobFileServices.DeleteFileAsync(thumbnailPath,cancellationToken);
            return Result.Ok();
        }
    }
}
