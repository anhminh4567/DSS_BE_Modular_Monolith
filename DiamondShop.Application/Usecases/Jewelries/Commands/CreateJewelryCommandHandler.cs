using DiamondShop.Application.Services.Data;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Repositories.JewelryRepo;
using MediatR;

namespace DiamondShop.Application.Usecases.Jewelries.Commands
{
    public record CreateJewelryCommand() : IRequest<Jewelry>;
    internal class CreateJewelryCommandHandler : IRequestHandler<CreateJewelryCommand, Jewelry>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateJewelryCommandHandler(IJewelryRepository jewelryRepository, IUnitOfWork unitOfWork)
        {
            _jewelryRepository = jewelryRepository;
            _unitOfWork = unitOfWork;
        }

        public Task<Jewelry> Handle(CreateJewelryCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
