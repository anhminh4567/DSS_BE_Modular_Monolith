using DiamondShop.Application.Commons.Models;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Transactions;
using FluentResults;

namespace DiamondShop.Application.Services.Interfaces.Transfers
{
    public interface ITransferFileService : IBlobFileServices
    {
        Task<Result> DeleteTransferImage(Transaction transaction, CancellationToken token = default);
        Task<List<Media>> GetTransferImage(Transaction transaction, CancellationToken token = default);
        Task<Result<string>> UploadTransferImage(Transaction transaction, FileData image, CancellationToken token = default);
    }
}
