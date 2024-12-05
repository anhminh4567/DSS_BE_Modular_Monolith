using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services.Payments.Vietqr.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Vietqr
{
    public class VietqrPaymentService
    {
        private const string GET_TOKEN = "https://dev.vietqr.org/vqr/api/token_generate";
        private const string username = "customer-testshop-user24200";
        private const string password = "Y3VzdG9tZXItdGVzdHNob3AtdXNlcjI0MjAw";
        private const string VALID_USERNAME = "Vnpay@12345";
        private const string VALID_PASSWORD = "Vnpay@12345"; // Base64 của username:password
        private const string SECRET_KEY = "your256bitsecretasdfaadsfasfasfsafsdafyour256bitsecretasdfaadsfasfasfsafsdaf"; // Bí mật để ký JWT token
        private const string BEARER_PREFIX = "Bearer ";
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IOptions<UrlOptions> _urlOptions;
        private static string SERVER_TRANS_SYNC = "https://<your-host>/vqr/bank/api/transaction-sync";
        private const string CREATE_PAYMENT_LINK = "https://dev.vietqr.org/vqr/api/qr/generate-customer";
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string BANKCODE = "MB";
        private const string BANKACCOUNT = "3080742980995";
        private const string USERBANKNAME = "TRAN PHAT DAT";
        public VietqrPaymentService(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IOptions<UrlOptions> urlOptions, IOrderRepository orderRepository, IHttpContextAccessor httpContextAccessor)
        {
            _optionsMonitor = optionsMonitor;
            _urlOptions = urlOptions;
            _orderRepository = orderRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public static async Task<string> GetToken()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(GET_TOKEN);
            var authValue = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            httpClient.DefaultRequestHeaders.Add("Authorization", authValue);
            var result = httpClient.PostAsync(GET_TOKEN,new StringContent("",MediaTypeHeaderValue.Parse("application/json"))).Result;
            if(result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadFromJsonAsync<VietQrAccessTokenResponse>();
                Console.WriteLine(content);
                return content.AccessToken;
            }
            else
            {
                var content = await result.Content.ReadFromJsonAsync<VietQrErrorResponse>();
                Console.WriteLine(content);
                return null;
            }
        }

        public static async Task<VietqrCreateResult> CreatePaymentLink(Order order, decimal amount)
        {
            var token = GetToken().Result;
            if (token is null)
                throw new Exception();
            var requestBody = new VietqrCreateBody()
            {
                //AdditionalInfo = order.OrderCode,
                amount = (long)amount,
                bankAccount = BANKACCOUNT,
                bankCode = BANKCODE,
                userBankName = USERBANKNAME,
                content = order.OrderCode,
                orderId = Guid.NewGuid().ToString().Substring(0,15),
                urlLink = "https://google.com",
                qrType = 0,
                transType = "C",


            };
            var httpClient = new HttpClient();
            //httpClient.BaseAddress = new Uri(CREATE_PAYMENT_LINK);
            //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var authValue = "Bearer " + token;
            var jsonBody = JsonConvert.SerializeObject(requestBody);
            httpClient.DefaultRequestHeaders.Add("Authorization", authValue);
            var result = httpClient.PostAsync(CREATE_PAYMENT_LINK, new StringContent(jsonBody, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"))).Result;
            if (result.IsSuccessStatusCode)
            {
                var body = await result.Content.ReadFromJsonAsync<VietqrCreateResult>();
                return body; ;
            }
            else
            {
                var body = await result.Content.ReadFromJsonAsync<VietqrCreateResponseError>();
                return null;
            }
        }
        public async Task<ActionResult> HandleCallback()
        {
            // Lấy token từ header Authorization
            var context = _httpContextAccessor.HttpContext;
            string authHeader = context.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith(BEARER_PREFIX))
            {
                var response = new ObjectResult( new VietqrErrorResponse
                {
                    Error = true,
                    ErrorReason = "INVALID_AUTH_HEADER",
                    ToastMessage = "Authorization header is missing or invalid",
                    Object = null
                });
                response.StatusCode = 401;
                return response;
            }

            string token = authHeader.Substring(BEARER_PREFIX.Length).Trim();

            // Xác thực token
            if (!ValidateToken(token))
            {
                var response = new ObjectResult(new VietqrErrorResponse
                {
                    Error = true,
                    ErrorReason = "INVALID_TOKEN",
                    ToastMessage = "Invalid or expired token",
                    Object = null
                });
                response.StatusCode = 401;
                return response;
            }

            // Xử lý logic của transaction
            try
            {
                // Ví dụ xử lý nghiệp vụ và sinh mã reftransactionid
                string refTransactionId = Guid.NewGuid().ToString(); // Tạo ID của giao dịch

                // Trả về response 200 OK với thông tin giao dịch
                var response =  new ObjectResult(new VietqrSuccessResponse
                {
                    Error = false,
                    ErrorReason = null,
                    ToastMessage = "Transaction processed successfully",
                    Object = new VietqrTransactionResponseObject
                    {
                        reftransactionid = refTransactionId
                    }
                });
                response.StatusCode = 200;
                return response;
            }
            catch (Exception ex)
            {
                // Trả về lỗi trong trường hợp có exception
                var response = new ObjectResult(new VietqrErrorResponse
                {
                    Error = true,
                    ErrorReason = "TRANSACTION_FAILED",
                    ToastMessage = ex.Message,
                    Object = null
                });
                response.StatusCode = 400;
                return response;
            }
        }
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
    }
}
