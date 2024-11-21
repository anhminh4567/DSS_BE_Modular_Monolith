using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.Jewelries.ErrorMessages;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Jewelries.Commands.Delete
{
    public record DeleteJewelryCommand(string JewelryId) : IRequest<Result>;
    internal class DeleteJewelryCommandHandler : IRequestHandler<DeleteJewelryCommand, Result>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteJewelryCommandHandler(IJewelryRepository jewelryRepository, IUnitOfWork unitOfWork)
        {
            _jewelryRepository = jewelryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteJewelryCommand request, CancellationToken token)
        {
            request.Deconstruct(out string modelId);
            var jewelry = await _jewelryRepository.GetById(JewelryId.Parse(modelId));
            if (jewelry == null)
                return Result.Fail(JewelryErrors.JewelryNotFoundError);
            var isExistingFlag = await _orderItemRepository.Existing(jewelry.Id);
            if (isExistingFlag)
                return Result.Fail(JewelryErrors.JewelryInUseError);
            //Delete gallery first

            await _unitOfWork.BeginTransactionAsync(token);
            await _jewelryRepository.Delete(jewelry, token);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return Result.Ok();
        }
    }
}
