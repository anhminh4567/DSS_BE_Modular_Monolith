using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Transfers;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Transactions.Enum;
using DiamondShop.Domain.Models.Transactions.ErrorMessages;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
using DiamondShop.Domain.Repositories.TransactionRepo;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DiamondShop.Api.Controllers.Orders
{
    public record DelivererChangeEvidenceCommand(string AccountId, ChangeEvidenceRequestDto ChangeEvidenceRequestDto) : IRequest<Result>;
    internal class DelivererChangeEvidenceCommandHandler : IRequestHandler<DelivererChangeEvidenceCommand, Result>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ITransferFileService _transferFileService;
        private readonly IUnitOfWork _unitOfWork;

        public DelivererChangeEvidenceCommandHandler(ITransactionRepository transactionRepository, ITransferFileService transferFileService, IUnitOfWork unitOfWork)
        {
            _transactionRepository = transactionRepository;
            _transferFileService = transferFileService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DelivererChangeEvidenceCommand request, CancellationToken token)
        {
            request.Deconstruct(out string accountId, out ChangeEvidenceRequestDto changeEvidenceRequestDto);
            await _unitOfWork.BeginTransactionAsync(token);
            changeEvidenceRequestDto.Deconstruct(out string transactionId, out IFormFile evidence);
            var transaction = await _transactionRepository.GetById(TransactionId.Parse(transactionId));
            if (transaction == null)
                return Result.Fail(TransactionErrors.TransactionNotFoundError);
            else if (transaction.Status != TransactionStatus.Verifying)
                return Result.Fail(TransactionErrors.TransferError.VerifiedError);
            else if (transaction.Status != TransactionStatus.Verifying)
                return Result.Fail(TransactionErrors.TransferError.EvidenceNotFoundError);
            else if (transaction.Order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            var order = transaction.Order;
            if(order.DelivererId != AccountId.Parse(accountId))
                return Result.Fail(OrderErrors.NoPermissionError);
            var deleteResult = await _transferFileService.DeleteTransferImage(transaction,token);
            if(deleteResult.IsFailed)
                return Result.Fail(deleteResult.Errors);
            var uploadResult = await _transferFileService.UploadTransferImage(transaction, new FileData(evidence.FileName, null, evidence.ContentType, evidence.OpenReadStream()));
            if (uploadResult.IsFailed)
                return Result.Fail(uploadResult.Errors);
            transaction.Evidence = Media.Create("evidence", uploadResult.Value, evidence.ContentType);
            await _transactionRepository.Update(transaction);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return Result.Ok();
        }
    }
}
