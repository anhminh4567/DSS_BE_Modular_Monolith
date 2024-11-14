using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using Quartz;

namespace DiamondShop.Infrastructure.BackgroundJobs
{
    public class CustomizeRequestWorker : IJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomizeRequestRepository _customizeRequestRepository;

        public CustomizeRequestWorker(IUnitOfWork unitOfWork, ICustomizeRequestRepository customizeRequestRepository)
        {
            _unitOfWork = unitOfWork;
            _customizeRequestRepository = customizeRequestRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var query = _customizeRequestRepository.GetQuery();
            query = query.Where(p =>
            p.Status != CustomizeRequestStatus.Shop_Rejected &&
            p.Status != CustomizeRequestStatus.Customer_Rejected &&
            p.ExpiredDate <= DateTime.UtcNow);
            if (query.Count() > 0)
            {
                var list = query.ToList();
                list.ForEach(p =>
                {
                    if (p.Status == CustomizeRequestStatus.Accepted)
                        p.Status = CustomizeRequestStatus.Customer_Rejected;
                    else
                        p.Status = CustomizeRequestStatus.Shop_Rejected;
                    });
                _customizeRequestRepository.UpdateRange(list);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
