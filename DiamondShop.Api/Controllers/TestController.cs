using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using DiamondShop.Domain.Models.AccountAggregate;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DiamondShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("zzz")]
    public class TestController : ApiControllerBase
    {
        private readonly IExternalBankServices _externalBankServices;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public TestController(IExternalBankServices externalBankServices, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _externalBankServices = externalBankServices;
            _optionsMonitor = optionsMonitor;
        }
        [HttpGet]
        public async Task<ActionResult> TestPaymentQrFromPayosVietQr()
        {
            var account = Account.Create(FullName.Create("a", "b"), "testingwebandstuff@gmail.com");
            var order = Order.Create(account.Id, PaymentType.Payall, PaymentMethodId.Parse("1"), 50_000_000, 20_000, 0, "abc", null, null, 40_000, 20_000, DateTime.UtcNow, OrderId.Parse("1"));
            order.Items.Add(OrderItem.Create(order.Id, null, DiamondId.Parse("1"), 25_000_000, 20_000_000, null, null, null, 0));
            order.Items.Add(OrderItem.Create(order.Id, JewelryId.Parse("2"), null, 25_000_000, 20_000_000, null, null, null, 0));
            return Ok( _externalBankServices.GenerateQrCodeFromOrder(order,50_000_000));
        }
        private const string VALID_USERNAME = "Vnpay@12345";
        private const string VALID_PASSWORD = "Vnpay@12345"; // Base64 của username:password
        private const string SECRET_KEY = "your256bitsecretasdfaadsfasfasfsafsdafyour256bitsecretasdfaadsfasfasfsafsdaf"; // Bí mật để ký JWT token

        [HttpPost("/vqr/api/token_generate")]
        [Produces("application/json")]
        public IActionResult GenerateToken([FromHeader] string Authorization)
        {
            // Kiểm tra Authorization header
            if (string.IsNullOrEmpty(Authorization) || !Authorization.StartsWith("Basic "))
            {
                return BadRequest("Authorization header is missing or invalid");
            }

            // Giải mã Base64
            var base64Credentials = Authorization.Substring("Basic ".Length).Trim();
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(base64Credentials));
            var values = credentials.Split(':', 2);

            if (values.Length != 2)
            {
                return BadRequest("Invalid Authorization header format");
            }

            var username = values[0];
            var password = values[1];

            // Kiểm tra username và password
            if (username == VALID_USERNAME && password == VALID_PASSWORD)
            {
                var token = GenerateJwtToken(username);
                return Ok(new
                {
                    access_token = token,
                    token_type = "Bearer",
                    expires_in = 300 // Thời gian hết hạn token
                });
            }
            else
            {
                return Unauthorized("Invalid credentials");
            }
        }

        // Hàm tạo JWT token
        private string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SECRET_KEY);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(5), // Token hết hạn sau 5 phút
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
