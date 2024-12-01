using DiamondShop.Application.Dtos.Responses.Notifications;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Notifications;
using DiamondShop.Domain.Models.Notifications.ValueObjects;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.OrderRepo;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace DiamondShop.Api.Controllers.Notifications
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        private readonly INotificationRepository _notificationRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;

        public NotificationController(ISender sender, IMapper mapper, INotificationRepository notificationRepository, IAccountRepository accountRepository, IUnitOfWork unitOfWork, IOrderRepository orderRepository)
        {
            _sender = sender;
            _mapper = mapper;
            _notificationRepository = notificationRepository;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result = await _notificationRepository.GetAll();
            var mappedResult = _mapper.Map<List<NotificationDto>>(result);
            return Ok(mappedResult);
        }
        [HttpGet("{accountId}")]
        public async Task<ActionResult> GetAccountMessaeg([FromRoute] string accountId)
        {
            var parsedId = AccountId.Parse(accountId);
            var getAccount = await _accountRepository.GetById(parsedId);
            if (getAccount == null)
            {
                return NotFound();
            }
            var result = await _notificationRepository.GetForUser(getAccount);
            var mappedResult = _mapper.Map<List<NotificationDto>>(result);
            return Ok(mappedResult);
        }
        [HttpGet("Shop/{orderId}")]
        public async Task<ActionResult> GetShopOrderMessaeg([FromRoute] string orderId)
        {
            var parsedId = OrderId.Parse(orderId);
            var getOrder = await _orderRepository.GetById(parsedId);
            if (getOrder == null)
            {
                return NotFound();
            }
            var result = await _notificationRepository.GetForOrder(getOrder);
            var mappedResult = _mapper.Map<List<NotificationDto>>(result);
            return Ok(mappedResult);
        }
        [HttpPost]
        public async Task<ActionResult> TestCreatePublicMessage([FromBody] string message)
        {
            Notification notification = Notification.CreatePublicMessage(message,null);
            _notificationRepository.Create(notification).Wait();
            _unitOfWork.SaveChangesAsync().Wait();
            return Ok(notification);
        }
        [HttpPut("{notificationId}")]
        public async Task<ActionResult> Read([FromRoute] string notificationId)
        {
            NotificationId id = NotificationId.Parse(notificationId);
            var tryGet = await _notificationRepository.GetById(id);
            ArgumentNullException.ThrowIfNull(tryGet);
            tryGet.Read();
            await _notificationRepository.Update(tryGet);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("{notificationId}")]
        public async Task<ActionResult> TestDeletePublicMessage([FromRoute] string notificationId)
        {
            NotificationId id = NotificationId.Parse(notificationId);
            var tryGet = await _notificationRepository.GetById(id);
            ArgumentNullException.ThrowIfNull(tryGet);
            _notificationRepository.Delete(tryGet).Wait();
            return Ok();

        }
    }
}
