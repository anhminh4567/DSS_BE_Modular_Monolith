using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders.Entities
{
    public class OrderLog : Entity<OrderLogId>
    {
        public OrderId OrderId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public OrderLogId? PreviousLogId { get; set; }
        public OrderLog? PreviousLog { get; set; }
        public OrderStatus Status { get; set; }
        public List<Media>? LogImages { get; set; }
        [NotMapped]
        public bool IsParentLog { get => PreviousLogId == null; }
        public static OrderLog CreateByChangeStatus(Order order, OrderStatus statusToChange) 
        {
            return new OrderLog
            {
                Id = OrderLogId.Create(),
                OrderId = order.Id,
                Message = $"Trạng thái sang {OrderStatusExtension.ToFriendlyString(statusToChange)} ",
                CreatedDate = DateTime.UtcNow,
                Status = statusToChange,
            };
        }
        public static OrderLog CreateProcessingLog(Order order, OrderLog parentLog, string message)
        {
            if (parentLog.Status != OrderStatus.Processing)
            {
                throw new InvalidOperationException("Cannot create Processing child log without Processing log");
            };
            return new OrderLog
            {
                Id = OrderLogId.Create(),
                OrderId = order.Id,
                Message = message,
                CreatedDate = DateTime.UtcNow,
                Status = OrderStatus.Processing,
                PreviousLogId = parentLog.Id,
            };
        }
        public static OrderLog CreateDeliveringLog(Order order, OrderLog parentLog, string message)
        {
            if(parentLog.Status != OrderStatus.Delivering)
            {
                throw new InvalidOperationException("Cannot create delivering child log without delivering log");
            };
            return new OrderLog
            {
                Id = OrderLogId.Create(),
                OrderId = order.Id,
                Message = message,
                CreatedDate = DateTime.UtcNow,
                Status = OrderStatus.Delivering,
                PreviousLogId = parentLog.Id,
            };

        }
        public void SetLogImages(List<Media> medias)
        {
            LogImages = medias;
        }
        public OrderLog() { }
    }
}
