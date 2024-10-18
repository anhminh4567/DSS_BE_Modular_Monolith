namespace DiamondShop.Application.Commons.Models
{
    public record FileData(string FileName, string? FileExtension, string contentType, Stream Stream);
}
