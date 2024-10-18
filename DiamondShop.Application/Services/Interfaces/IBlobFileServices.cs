using DiamondShop.Domain.Common.ValueObjects;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces
{
    public interface IBlobFileServices
    {
        Task<Result> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);
        Task<Result<BlobFileResponseDto>> DownloadFileAsync(string filePath, CancellationToken cancellationToken = default);
        Task<Result<string>> UploadFileAsync(string filePath, Stream stream, string contentType, CancellationToken cancellationToken = default);
        Task<List<Media>> GetFolders(string folderPath, CancellationToken cancellationToken = default);
        string ToAbsolutePath(string relativePath);
        string ToRelativePath(string absolutePath);
    }
    public class BlobFileResponseDto
    {
        public Stream Stream { get; set; }
        public string ContentType { get; set; }
    }
    public enum BlobDirectoryType
    {
        Public, PaidContent, Private
    }
}
