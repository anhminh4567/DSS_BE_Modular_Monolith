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
using OpenQA.Selenium;
using DiamondShop.Infrastructure.Services.Payments.Vietqr.Models;
using DiamondShop.Infrastructure.Services.Payments.Vietqr;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;

namespace DiamondShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("zzz")]
    public class TestController : ApiControllerBase
    {
        private readonly IExternalBankServices _externalBankServices;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private const string VALID_USERNAME = "Vnpay@12345";
        private const string VALID_PASSWORD = "Vnpay@12345"; // Base64 của username:password
        private const string SECRET_KEY = "your256bitsecretasdfaadsfasfasfsafsdafyour256bitsecretasdfaadsfasfasfsafsdaf"; // Bí mật để ký JWT token
        private const string BEARER_PREFIX = "Bearer ";
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
            order.Items.Add(OrderItem.Create(order.Id, "", null, DiamondId.Parse("1"), 25_000_000, 20_000_000, null, null, null, 0));
            order.Items.Add(OrderItem.Create(order.Id, "", JewelryId.Parse("2"), null, 25_000_000, 20_000_000, null, null, null, 0));
            return Ok(_externalBankServices.GenerateQrCodeFromOrder(order, 50_000_000));
        }

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
        [HttpPost("testcreate")]
        public async Task<ActionResult> CreatePaymentQr()
        {
            var account = Account.Create(FullName.Create("a", "b"), "testingwebandstuff@gmail.com");
            var order = Order.Create(account.Id, PaymentType.Payall, PaymentMethodId.Parse("1"), 50_000_000, 20_000, 0.312m, "abc", null, null, 40_000, 20_000, DateTime.UtcNow, OrderId.Parse("1"));
            var diamond = OrderItem.Create(order.Id, "", null, DiamondId.Parse("1"), 25_000_000, 20_000_000, null, null, null, 0);
            var jewellry = OrderItem.Create(order.Id, "", JewelryId.Parse("2"), null, 25_000_000, 20_000_000, null, null, null, 0);
            diamond.Diamond = Diamond.Create(DiamondShape.ROUND, new Diamond_4C(Cut.Excellent, Color.I, Clarity.FL, 0.22f, true), new Diamond_Details(Polish.Excellent, Symmetry.Excellent, Girdle.Thick, Fluorescence.Faint, Culet.Medium), new Diamond_Measurement(0.22f, 23f, 23f, "asdf"), 0.3m, "sfd");
            jewellry.Jewelry = Jewelry.Create(JewelryModelId.Create(), SizeId.Create(), MetalId.Create(), 2f, "sdf", DiamondShop.Domain.Common.Enums.ProductStatus.Active);
            order.Items.Add(diamond);

            order.Items.Add(jewellry);
            var amount = 10000;
            var result = VietqrPaymentService.CreatePaymentLink(order,amount).Result;
            return Ok(result);
        }

        // API để xử lý transaction-sync

        [HttpPost("/vqr/bank/api/transaction-sync")]
        public IActionResult TransactionSync([FromBody] VietqrTransactionCallback transactionCallback)
        {
            // Lấy token từ header Authorization
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith(BEARER_PREFIX))
            {
                return StatusCode(401, new VietqrErrorResponse
                {
                    Error = true,
                    ErrorReason = "INVALID_AUTH_HEADER",
                    ToastMessage = "Authorization header is missing or invalid",
                    Object = null
                });
            }

            string token = authHeader.Substring(BEARER_PREFIX.Length).Trim();

            // Xác thực token
            if (!ValidateToken(token))
            {
                return StatusCode(401, new VietqrErrorResponse
                {
                    Error = true,
                    ErrorReason = "INVALID_TOKEN",
                    ToastMessage = "Invalid or expired token",
                    Object = null
                });
            }

            // Xử lý logic của transaction
            try
            {
                // Ví dụ xử lý nghiệp vụ và sinh mã reftransactionid
                string refTransactionId = "GeneratedRefTransactionId"; // Tạo ID của giao dịch

                // Trả về response 200 OK với thông tin giao dịch
                return Ok(new VietqrSuccessResponse
                {
                    Error = false,
                    ErrorReason = null,
                    ToastMessage = "Transaction processed successfully",
                    Object = new VietqrTransactionResponseObject
                    {
                        reftransactionid = refTransactionId
                    }
                });
            }
            catch (Exception ex)
            {
                // Trả về lỗi trong trường hợp có exception
                return StatusCode(400, new VietqrErrorResponse
                {
                    Error = true,
                    ErrorReason = "TRANSACTION_FAILED",
                    ToastMessage = ex.Message,
                    Object = null
                });
            }
        }

        // Phương thức để xác thực token JWT
        private bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SECRET_KEY);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero, // Không cho phép độ trễ thời gian
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
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
