using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.Notifications;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Application.Dtos.Responses.Orders;
using DiamondShop.Domain;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;

namespace DiamondShop.Application.Dtos.Responses.Notifications
{
    public class NotificationDto
    {
        public string Id { get; set; }
        public string? AccountId { get; set; }
        public string? OrderId { get; set; }
        public OrderDto? Order { get; set; }
        public string Content { get; set; }
        public string? ContentType { get; set; }
        public string CreatedDate { get; set; }
        public bool IsRead { get; set; }
        public object? MessageObject
        {
            get
            {
                if (ContentType != null)
                {
                    var messageTypee = System.Type.GetType(ContentType);
                    if (messageTypee == null)
                        return null;
                    var parsedObject = JsonConvert.DeserializeObject(Content, messageTypee);
                    return parsedObject;
                }
                return null;
            }
        }
    }
}
