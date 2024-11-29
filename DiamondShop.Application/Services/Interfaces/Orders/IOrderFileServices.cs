using DiamondShop.Application.Commons.Models;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Transactions;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces.Orders
{
    public interface IOrderFileServices : IBlobFileServices
    {
        Task<Result<string[]>> UploadOrderLogImage(Order order, OrderLog log, FileData[] images, CancellationToken cancellationToken = default);
        Task<Result<string[]>> UploadOrderTransactionImage(Order order, Transaction transaction, FileData[] images, CancellationToken cancellationToken = default);        
        Task<OrderGalleryTemplate> GetAllOrderImages(Order order, CancellationToken cancellationToken = default);
        Task<Result<List<Media>>> GetOrderLogImages(Order order, OrderLog orderLog, CancellationToken cancellationToken = default);
        Task<Result<List<Media>>> GetOrderTransactionImages(Order order, Transaction transaction, CancellationToken cancellationToken = default);

        Task<Result<Media>> CreateOrderInvoice(Order fullDetailOrder, CancellationToken cancellationToken = default);
        Task<Result<Media>> GetOrCreateOrderInvoice(Order fullDetailOrder, CancellationToken cancellationToken = default);
        Task<Result<List<Media>>> SaveOrderConfirmDeliveryImage(Order finishDeliveryOrder, List<FileData> images);
        Task<Result<Media>> SaveOrderConfirmVideo(Order fnishDeliveryOrder, FileData video);
    }
}
