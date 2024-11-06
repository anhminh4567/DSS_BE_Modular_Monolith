using DiamondShop.Application.Commons.Utilities;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Diamonds;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.DiamondPrices.Commands.UpdateThroughExcels
{
    //return the excel file read from the FormFile
    public record UpdateDiamondPriceThroughExcelFileCommand(IFormFile excelFile) : IRequest<Result<Stream>>;
    internal class UpdateDiamondPriceThroughExcelFileCommandHandler : IRequestHandler<UpdateDiamondPriceThroughExcelFileCommand, Result<Stream>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        private readonly IDiamondExcelService _diamondExcelService;
        private readonly IExcelService _excelService;

        public UpdateDiamondPriceThroughExcelFileCommandHandler(IUnitOfWork unitOfWork, IDiamondPriceRepository diamondPriceRepository, IDiamondCriteriaRepository diamondCriteriaRepository, IDiamondExcelService diamondExcelService, IExcelService excelService)
        {
            _unitOfWork = unitOfWork;
            _diamondPriceRepository = diamondPriceRepository;
            _diamondCriteriaRepository = diamondCriteriaRepository;
            _diamondExcelService = diamondExcelService;
            _excelService = excelService;
        }

        public async Task<Result<Stream>> Handle(UpdateDiamondPriceThroughExcelFileCommand request, CancellationToken cancellationToken)
        {
            var fileExtension = Path.GetExtension(request.excelFile.FileName).Trim();
            var contentType = request.excelFile.ContentType;
            var isExcelFile = FileUltilities.IsExcelFileExtension(fileExtension);
            if (isExcelFile != null)
                return Result.Fail(isExcelFile);
            throw new Exception();
        }
    }
}
