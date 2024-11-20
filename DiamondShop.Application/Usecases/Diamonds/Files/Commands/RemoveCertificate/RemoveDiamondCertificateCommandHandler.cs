using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Diamonds;
using DiamondShop.Domain.Models.Diamonds;
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

namespace DiamondShop.Application.Usecases.Diamonds.Files.Commands.RemoveCertificate
{
    public record RemoveDiamondCertificateCommand(string DiamondId, string certificateAbsolutePath) : IRequest<Result>;
    internal class RemoveDiamondCertificateCommandHandler : IRequestHandler<RemoveDiamondCertificateCommand, Result>
    {
        private readonly IDiamondFileService _diamondFileService;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveDiamondCertificateCommandHandler(IDiamondFileService diamondFileService, IDiamondRepository diamondRepository, IUnitOfWork unitOfWork)
        {
            _diamondFileService = diamondFileService;
            _diamondRepository = diamondRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemoveDiamondCertificateCommand request, CancellationToken cancellationToken)
        {
            var parsedId = DiamondId.Parse(request.DiamondId);
            var getDiamond = await _diamondRepository.GetById(parsedId);
            if(getDiamond == null)
            {
                return Result.Fail(DiamondErrors.DiamondNotFoundError);
            }
            string relativePath =  _diamondFileService.ToRelativePath(request.certificateAbsolutePath);
            var removeResult = await _diamondFileService.DeleteFileAsync(relativePath, cancellationToken);
            getDiamond.SetCertificate(null,null);
            return removeResult;
        }
    }
    
}
