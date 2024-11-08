using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.JewelryReview;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.JewelryReviewRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DiamondShop.Application.Usecases.JewelryReviews.Commands.Create
{
    public record JewelryReviewRequestDto(string JewelryId, string Content, int starRating, IFormFile[] Files);
    public record CreateJewelryReviewCommand(string AccountId, JewelryReviewRequestDto JewelryReviewRequestDto) : IRequest<Result<JewelryReview>>;
    internal class CreateJewelryReviewCommandHandler : IRequestHandler<CreateJewelryReviewCommand, Result<JewelryReview>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IJewelryReviewRepository _jewelryReviewRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJewelryReviewFileService _jewelryReviewFileService;

        public CreateJewelryReviewCommandHandler(IJewelryReviewRepository jewelryReviewRepository, IUnitOfWork unitOfWork, IAccountRepository accountRepository, IOrderRepository orderRepository, IJewelryRepository jewelryRepository, IJewelryReviewFileService jewelryReviewFileService)
        {
            _jewelryReviewRepository = jewelryReviewRepository;
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _orderRepository = orderRepository;
            _jewelryRepository = jewelryRepository;
            _jewelryReviewFileService = jewelryReviewFileService;
        }

        public async Task<Result<JewelryReview>> Handle(CreateJewelryReviewCommand request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out string accountId, out JewelryReviewRequestDto jewelryReviewRequestDto);
            jewelryReviewRequestDto.Deconstruct(out string jewelryId, out string content, out int starRating, out IFormFile[] files);
            var account = await _accountRepository.GetById(AccountId.Parse(accountId));
            if (account == null)
                return Result.Fail("Can't get account information");
            //TODO: Add check permission
            var jewelry = await _jewelryRepository.GetById(JewelryId.Parse(jewelryId));
            if (jewelry == null)
                return Result.Fail("Can't get the jewelry information");

            var checkOwner = await _orderRepository.IsOwner(account.Id, jewelry.Id);
            if (!checkOwner)
                return Result.Fail("You don't have permission to create review for this jewelry");
            var checkExist = await _jewelryReviewRepository.GetById(jewelry.Id);
            if (checkExist != null)
                return Result.Fail("You have already created a review for this jewelry");

            //Add images
            FileData[] fileDatas = files.Select(f => new FileData(f.Name, f.ContentType, f.ContentType, f.OpenReadStream())).ToArray();
            await _jewelryReviewFileService.UploadReview(jewelry, fileDatas);
            

            JewelryReview review = JewelryReview.Create(jewelry.Id, account.Id, content, starRating, null);
            throw new NotImplementedException();
        }
    }
}
