using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Commons.Utilities;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Diamonds;
using DiamondShop.Application.Services.Models;
using DiamondShop.Commons;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Files.Commands.AddCertficate
{
    public record class AddDiamondCertificateCommand(string diamondId, IFormFile pdfFile) : IRequest<Result<Media>>;
    internal class AddDiamondCertificateCommandHandler : IRequestHandler<AddDiamondCertificateCommand, Result<Media>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondFileService _diamondFileService;

        public AddDiamondCertificateCommandHandler(IUnitOfWork unitOfWork, IDiamondRepository diamondRepository, IDiamondFileService diamondFileService)
        {
            _unitOfWork = unitOfWork;
            _diamondRepository = diamondRepository;
            _diamondFileService = diamondFileService;
        }

        public async Task<Result<Media>> Handle(AddDiamondCertificateCommand request, CancellationToken cancellationToken)
        {
            var parsedId = DiamondId.Parse(request.diamondId);
            var getDiamond = await _diamondRepository.GetById(parsedId);
            if (getDiamond == null)
                return Result.Fail(new NotFoundError());
            var fileExtension = Path.GetExtension(request.pdfFile.FileName).Trim();
            var fileName = request.pdfFile.FileName.Replace(fileExtension, "");
            if (FileUltilities.IsPdfFileContentType(request.pdfFile.ContentType) == false ||
                FileUltilities.IsPdfFileExtension(fileExtension) == false)
                return Result.Fail(new ConflictError("file is not type PDF")) ;

            var stream = request.pdfFile.OpenReadStream();
            var pdfObject = new FileData(fileName,fileExtension,request.pdfFile.ContentType,stream);
            var uploadResult = await _diamondFileService.UploadCertificatePdf(getDiamond, pdfObject, cancellationToken);
            if (uploadResult.IsSuccess) 
            {
                return Result.Ok(Media.Create(fileName, uploadResult.Value, request.pdfFile.ContentType));
            }
            return Result.Fail(uploadResult.Errors);
        }
    }
}
