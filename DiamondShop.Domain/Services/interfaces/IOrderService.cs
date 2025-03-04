﻿using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories;
using FluentResults;
using DiamondShop.Domain.Models.Warranties.Enum;

namespace DiamondShop.Domain.Services.interfaces
{
    public interface IOrderService
    {
        Result CheckWarranty(string? jewelryId, string? diamondId, WarrantyType warrantyType);
        Task<Result<Order>> AssignDeliverer(Order order, string delivererId);
        public bool IsCancellable(OrderStatus order);
        public bool IsProceedable(OrderStatus order);
        public bool IsDeliverCancellable(OrderStatus order);
        public bool CheckForSameCity(List<Order> orders);
        public Task<Result> CancelItems(Order order);
    }
}
