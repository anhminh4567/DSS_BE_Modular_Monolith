using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Commons.Utilities;
using DiamondShop.Application.Services.Interfaces.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Diamonds.Files.Queries
{
    // chi duoc lay certficate cua diamond cua minh va order phai la success
    public record GetDiamondCertificatesQuery(string diamondCode, string orderCode) : IRequest<Result<FileDownloadData>>;
    internal class GetDiamondCertificatesQueryHandler : IRequestHandler<GetDiamondCertificatesQuery, Result<FileDownloadData>>
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDiamondFileService _diamondFileService;

        public GetDiamondCertificatesQueryHandler(IDiamondRepository diamondRepository, IOrderRepository orderRepository, IAccountRepository accountRepository, IHttpContextAccessor httpContextAccessor, IDiamondFileService diamondFileService)
        {
            _diamondRepository = diamondRepository;
            _orderRepository = orderRepository;
            _accountRepository = accountRepository;
            _httpContextAccessor = httpContextAccessor;
            _diamondFileService = diamondFileService;
        }

        public async Task<Result<FileDownloadData>> Handle(GetDiamondCertificatesQuery request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if(httpContext == null)
                return Result.Fail(HttpContextExtensions.CommonErrors.NotFound);
            if (httpContext.User.Identity == null)
                return Result.Fail(HttpContextExtensions.CommonErrors.NotAuthenticated);
            if(httpContext.User.Identity.IsAuthenticated == false)
                return Result.Fail(HttpContextExtensions.CommonErrors.NotAuthenticated);
            var order = await _orderRepository.GetOrderByCode(request.orderCode);
            if (order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            var diamondItems = order.Items.Where(x => x.DiamondId != null);
            if(diamondItems.Count() ==0)
                return Result.Fail(DiamondErrors.DiamondNotFoundError);
            var askedDiamond = diamondItems.Select(x => x.Diamond).Where(x => x.SerialCode == request.diamondCode).FirstOrDefault();
            if(askedDiamond == null)
                return Result.Fail(DiamondErrors.DiamondNotFoundError);
            if(askedDiamond.CertificateCode == null || askedDiamond.CertificateFilePath == null)
                return Result.Fail(DiamondErrors.NotHavingCertificate);
            var downloadedFIle = await _diamondFileService.DownloadFileAsync(askedDiamond.CertificateFilePath.MediaPath);
            if(downloadedFIle.IsFailed)
                return Result.Fail(downloadedFIle.Errors);
            var fileName = "CertificateDiamond";
            var fileExtension = "pdf";
            var mimeType = "application/pdf";
            var timeStampe = DateTime.Now;
            FileDownloadData fileDownloadData = new(fileName,fileExtension,downloadedFIle.Value.ContentType,timeStampe,downloadedFIle.Value.Stream);
            return fileDownloadData;
        }
    }
}
