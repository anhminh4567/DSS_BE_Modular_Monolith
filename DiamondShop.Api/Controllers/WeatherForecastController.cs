using DiamondShop.Api.Controllers;
using DiamondShop.Application.Dtos.Requests.Deliveries;
using DiamondShop.Application.Dtos.Requests.Diamonds;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Models;
using DiamondShop.Application.Usecases.DeliveryFees.Commands.CreateMany;
using DiamondShop.Application.Usecases.DiamondCriterias.Commands.CreateMany;
using DiamondShop.Application.Usecases.DiamondPrices.Commands.CreateMany;
using DiamondShop.Application.Usecases.DiamondShapes.Queries.GetAll;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.DeliveryFees;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories.DeliveryRepo;
using DiamondShop.Infrastructure.Databases;
using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services.Excels;
using DiamondShop.Infrastructure.Services.Locations.Locally;
using DiamondShop.Infrastructure.Services.Payments.Paypals;
using DiamondShop.Infrastructure.Services.Payments.Paypals.Models;
using DiamondShop.Infrastructure.Services.Payments.Vnpays;
using DiamondShop.Infrastructure.Services.Scrapers;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace DiamondShopSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ApiControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IBlobFileServices _blobFileServices;
        private readonly IOptions<PaypalOption> _paypal;
        private readonly IOptions<VnpayOption> _vnpayOption;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISender _sender;
        private readonly IOptions<LocationOptions> _locationOptions;
        private readonly DiamondShopDbContext _context;
        private readonly ILocationService _locationService;
        private readonly IDeliveryFeeRepository _deliveryFeeRepository;
        private readonly IEmailService _emailService;
        private readonly IPaymentService _paymentService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IDateTimeProvider dateTimeProvider, IBlobFileServices blobFileServices, IOptions<PaypalOption> paypal, IOptions<VnpayOption> vnpayOption, IHttpContextAccessor httpContextAccessor, ISender sender, IOptions<LocationOptions> locationOptions, DiamondShopDbContext context, ILocationService locationService, IDeliveryFeeRepository deliveryFeeRepository, IEmailService emailService, IPaymentService paymentService)
        {
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
            _blobFileServices = blobFileServices;
            _paypal = paypal;
            _vnpayOption = vnpayOption;
            _httpContextAccessor = httpContextAccessor;
            _sender = sender;
            _locationOptions = locationOptions;
            _context = context;
            _locationService = locationService;
            _deliveryFeeRepository = deliveryFeeRepository;
            _emailService = emailService;
            _paymentService = paymentService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet("GetTimeZone")]
        public ActionResult GetTimeZone()
        {
            return Ok(TimeZoneInfo.Local);
        }
        [HttpPost("upload-file")]
        public async Task<ActionResult> Upload(IFormFile files)
        {
            using Stream openstream = files.OpenReadStream();
            var result = await _blobFileServices.UploadFileAsync("fakefileupload/fakefile", openstream, files.ContentType);

            return Ok();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("delete-file")]
        public async Task<ActionResult> download()
        {
            var result = await _blobFileServices.DeleteFileAsync("fakefileupload/fakefile");
            return Ok();
        }
        [HttpPost("test-excel")]
        public async Task<ActionResult> Excel(IFormFile formFile)
        {
            var stream = formFile.OpenReadStream();
            var test = new ExcelSyncfunctionService();
            var app = ExcelSyncfunctionService.GetExcelApplication();
            var workbook = app.OpenWorkBook(stream, formFile.FileName);
            var worksheet = workbook.Worksheets.First();
            var rowCount = worksheet.Rows.Length;
            List<CaohungDiamondPrice> listItem = new();
            for (int i = 0; i < rowCount; i++)
            {
                if (i == 0)
                    continue; //skip the first row, the header
                var obj = ExcelSyncFunctionExtension.ReadLine<CaohungDiamondPrice>(worksheet, i, 0);
                listItem.Add(obj);
            }

            // Map the listItem to another list
            var mappedList = listItem.Select(item => new DiamondCriteriaRequestDto()
            {
                CaratFrom = ((float)item.CaratFrom),
                CaratTo = (float)item.CaratTo,
                Clarity = item.Clarity,
                Color = item.Color,
                Cut = item.Cut,
            }).ToList();
            var prices = listItem.Select(item => item.Price).ToList();
            // Dispose the listItem to free up memory
            listItem.Clear();
            listItem = null;
            var result = await _sender.Send(new CreateManyDiamondCriteriasCommand(mappedList));
            var getShapes = await _sender.Send(new GetAllDiamondShapeQuery());
            var round = getShapes.FirstOrDefault(item => item.Shape.ToUpper() == "ROUND");
            var pear = getShapes.FirstOrDefault(item => item.Shape.ToUpper() == "PEAR");
            var mappedPriceList = result.Value.Select((item, index) => new DiamondPriceRequestDto(item.Id, round.Id, prices[index],true)).ToList();
            var mappedPriceList2 = result.Value.Select((item, index) => new DiamondPriceRequestDto(item.Id, pear.Id, prices[index], true)).ToList();

            var result2 = await _sender.Send(new CreateManyDiamondPricesCommand(mappedPriceList));
            var result3 = await _sender.Send(new CreateManyDiamondPricesCommand(mappedPriceList2));

            return Ok();
        }
        [Route("/callback-ngrok")]
        [HttpGet]
        public async Task<ActionResult> CallbackNgrokTest()
        {
            Console.WriteLine("Hello world, calling from vnpay");
            return Ok();
        }
        [HttpPost("/ThemTinhThanh")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Insert63CityLocation()
        {
            var getCities = _locationService.GetProvinces();
            var shopCity = getCities.FirstOrDefault(x => x.Name == _locationOptions.Value.ShopOrignalLocation.OriginalProvince);
            if (shopCity == null)
                throw new Exception();
            var baseCost = 40000.0m;
            List<CreateDeliveryFeeCommand> commandData = new();
            foreach (var city in getCities)
            {
                baseCost += 5000.0m;
                var comando = new CreateDeliveryFeeCommand(DeliveryFeeType.LocationToCity, city.Name, baseCost, new ToLocationCity(shopCity.Name, city.Name), null);
                commandData.Add(comando);
            }
            var result = await _sender.Send(new CreateManyDeliveryFeeCommand(commandData));
            return Ok(result.Value);
        }
        [HttpGet("testemail")]
        public async Task<ActionResult> test()
        {
            await _emailService.SendConfirmAccountEmail(Account.CreateBaseCustomer(FullName.Create("1232", "123"), "testingwebandstuff@gmail.com", "sdfasdf", new List<AccountRole>() { AccountRole.Customer }), "testtoken");
            return Ok();
        }
        [HttpGet("testzalopayservice")]
        public async Task<ActionResult> testzalopayservice()
        {
            Account falseAccount = Account.CreateBaseCustomer(FullName.Create("minh", "tran"), "abc@gmail.com", "asdf", new List<AccountRole> { AccountRole.Customer });
            Order falseOrder = Order.Create(falseAccount.Id, PaymentType.Payall, 100000m, 10000m, "adfads");
            PaymentLinkRequest falseRequest = new PaymentLinkRequest()
            {
                Account = falseAccount,
                Address = falseOrder.ShippingAddress,
                Amount = falseOrder.TotalPrice,
                Email = falseAccount.Email,
                Order = falseOrder,
                Title = "Test",
            };
            var reresult = await _paymentService.CreatePaymentLink(falseRequest);
            return Ok(reresult.Value);
        }
        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("seedcriteria")]
        public async Task<ActionResult> SeedCriteria()
        {
            var colorEnums = Enum.GetValues(typeof(Color));
            var clarityEnums = Enum.GetValues(typeof(Clarity));
            Cut defaultCut = Cut.Excelent;
            decimal startPrice = 30_000;//vnd
            List<DiamondShape> getShapes = await _sender.Send(new GetAllDiamondShapeQuery());
            List<(float caratFrom, float caratTo)> caratRange = new()
            {
                new (0.01f, 0.03f),
                new (0.03f, 0.07f),
                new (0.07f, 0.14f),
                new (0.14f, 0.17f),
                new (0.17f, 0.22f),
                new (0.22f, 0.29f),
                new (0.22f, 0.29f),
                new (0.29f, 0.39f),
                new (0.39f, 0.49f),
                new (0.49f, 0.69f),
                new (0.69f, 0.89f),
                new (0.89f, 0.99f),
                new (0.99f, 1.49f),
                new (1.49f, 1.99f),
            };
            var rowIncrementPrice = 10_000;
            var columnIncrementPrice = 5_000;
            var caratIncrementPrice = 10_000;
            foreach(var carat in caratRange)
            {
                List<DiamondCriteriaRequestDto> diamondCriteriaRequestDtos = new();
                List<decimal> prices = new();
                var basePrice = (decimal)(carat.caratFrom * 100) * (startPrice * (decimal)(carat.caratTo * 100));
                for (int i = 0; i < colorEnums.Length; i++)
                {
                    var color = (Color)colorEnums.GetValue(i);
                    var rowPrice = basePrice + rowIncrementPrice * i;
                    for (int j = 0; j < clarityEnums.Length; j++)
                    {
                        var clarity = (Clarity)clarityEnums.GetValue(j);
                        var columnPrice = rowPrice + columnIncrementPrice * j;
                        diamondCriteriaRequestDtos.Add(new DiamondCriteriaRequestDto()
                        {
                            CaratFrom = ((float)carat.caratFrom),
                            CaratTo = (float)carat.caratTo,
                            Clarity = clarity,
                            Color = color,
                            Cut = defaultCut,
                        });
                        prices.Add(columnPrice);
                    }
                }
                var result = await _sender.Send(new CreateManyDiamondCriteriasCommand(diamondCriteriaRequestDtos));
                foreach(var shape in getShapes)
                {
                    var mappedListDiamondLab = result.Value.Select((item, index) => new DiamondPriceRequestDto(item.Id, shape.Id, prices[index], true)).ToList();
                    var mappedListNatural = result.Value.Select((item, index) => new DiamondPriceRequestDto(item.Id, shape.Id, prices[index], false)).ToList();

                    var resultLab = await _sender.Send(new CreateManyDiamondPricesCommand(mappedListDiamondLab));
                    var resultNatural = await _sender.Send(new CreateManyDiamondPricesCommand(mappedListNatural));

                }
            }
            
            //var result = await _sender.Send(new CreateManyDiamondCriteriasCommand(mappedList));
            //var getShapes = await _sender.Send(new GetAllDiamondShapeQuery());
            //var round = getShapes.FirstOrDefault(item => item.Shape.ToUpper() == "ROUND");
            //var pear = getShapes.FirstOrDefault(item => item.Shape.ToUpper() == "PEAR");
            //var mappedPriceList = result.Value.Select((item, index) => new DiamondPriceRequestDto(item.Id, round.Id, prices[index])).ToList();
            //var mappedPriceList2 = result.Value.Select((item, index) => new DiamondPriceRequestDto(item.Id, pear.Id, prices[index])).ToList();

            //var result2 = await _sender.Send(new CreateManyDiamondPricesCommand(mappedPriceList));
            //var result3 = await _sender.Send(new CreateManyDiamondPricesCommand(mappedPriceList2));
            return Ok();
        }
    }
}
