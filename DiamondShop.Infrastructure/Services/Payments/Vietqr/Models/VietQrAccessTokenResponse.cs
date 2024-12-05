using System.Text.Json.Serialization;

namespace DiamondShop.Infrastructure.Services.Payments.Vietqr.Models
{
    public record VietQrAccessTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; }
        [JsonPropertyName("token_type")]
        public string TokenType { get; init; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; }
    }
}
