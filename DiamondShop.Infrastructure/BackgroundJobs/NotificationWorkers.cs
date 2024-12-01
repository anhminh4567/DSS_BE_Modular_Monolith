using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.Notifications;
using DiamondShop.Domain.Repositories;
using DiamondShop.Infrastructure.Options;
using MediatR;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.BackgroundJobs
{
    internal class NotificationWorkers : IJob
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IOptions<InAppNotificationOptions> _options;

        public NotificationWorkers(INotificationRepository notificationRepository, IUnitOfWork unitOfWork, IMediator mediator, IOptions<InAppNotificationOptions> options)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _options = options;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            //await RemoveExpiredNotification(context);
        }
        private async Task RemoveExpiredNotification(IJobExecutionContext context)
        {
            DateTime utcNow = DateTime.UtcNow;
            var publicMessageExpiredDate = utcNow.AddHours(-_options.Value.PublicMessageExpiredHour);
            var shopMessageExpiredDate = utcNow.AddHours(-_options.Value.ShopMessageExpiredHour);
            var accountMessageExpiredDate = utcNow.AddHours(-_options.Value.AccountMessageExpiredHour);
            Expression< Func<Notification, bool>> predicateToDelete;
            //for readed message
            predicateToDelete = x => x.IsRead;
            await _notificationRepository.DeleteBulk(predicateToDelete);
            //for public messages
            predicateToDelete = x => x.AccountId == null && x.OrderId == null && x.CreatedDate <= publicMessageExpiredDate;
            await _notificationRepository.DeleteBulk(predicateToDelete);
            //for shop messages
            predicateToDelete = x => x.AccountId == null && x.OrderId != null && x.CreatedDate <= shopMessageExpiredDate;
            await _notificationRepository.DeleteBulk(predicateToDelete);
            //for account messages
            predicateToDelete = x => x.AccountId != null && x.OrderId != null && x.CreatedDate <= accountMessageExpiredDate;
            await _notificationRepository.DeleteBulk(predicateToDelete);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
