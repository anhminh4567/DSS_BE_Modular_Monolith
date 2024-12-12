using DiamondShop.Api.Controllers;
using DiamondShop.Application.Dtos.Requests.Diamonds;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Models;
using DiamondShop.Application.Usecases.DeliveryFees.Commands.CreateMany;
using DiamondShop.Application.Usecases.DiamondCriterias.Commands.CreateFromRange;
using DiamondShop.Application.Usecases.DiamondPrices.Commands.CreateMany;
using DiamondShop.Application.Usecases.Jewelries.Commands.Seeding;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
using DiamondShop.Domain.Repositories.DeliveryRepo;
using DiamondShop.Infrastructure.Databases;
using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services.Pdfs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Diagnostics;

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
        private readonly DiamondShopDbContext _dbContext;
        private readonly ILocationService _locationService;
        private readonly IDeliveryFeeRepository _deliveryFeeRepository;
        private readonly IEmailService _emailService;
        private readonly IPaymentService _paymentService;
        private readonly IPdfService _pdfService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IDateTimeProvider dateTimeProvider, IBlobFileServices blobFileServices, IOptions<PaypalOption> paypal, IOptions<VnpayOption> vnpayOption, IHttpContextAccessor httpContextAccessor, ISender sender, IOptions<LocationOptions> locationOptions, DiamondShopDbContext dbContext, ILocationService locationService, IDeliveryFeeRepository deliveryFeeRepository, IEmailService emailService, IPaymentService paymentService, IPdfService pdfService)
        {
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
            _blobFileServices = blobFileServices;
            _paypal = paypal;
            _vnpayOption = vnpayOption;
            _httpContextAccessor = httpContextAccessor;
            _sender = sender;
            _locationOptions = locationOptions;
            _dbContext = dbContext;
            _locationService = locationService;
            _deliveryFeeRepository = deliveryFeeRepository;
            _emailService = emailService;
            _paymentService = paymentService;
            _pdfService = pdfService;
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
            //var stream = formFile.OpenReadStream();
            //var test = new ExcelSyncfunctionService();
            //var app = ExcelSyncfunctionService.GetExcelApplication();
            //var workbook = app.OpenWorkBook(stream, formFile.FileName);
            //var worksheet = workbook.Worksheets.First();
            //var rowCount = worksheet.Rows.Length;
            //List<CaohungDiamondPrice> listItem = new();
            //for (int i = 0; i < rowCount; i++)
            //{
            //    if (i == 0)
            //        continue; //skip the first row, the header
            //    var obj = ExcelSyncFunctionExtension.ReadLine<CaohungDiamondPrice>(worksheet, i, 0);
            //    listItem.Add(obj);
            //}

            //// Map the listItem to another list
            //var mappedList = listItem.Select(item => new DiamondCriteriaRequestDto()
            //{
            //    CaratFrom = ((float)item.CaratFrom),
            //    CaratTo = (float)item.CaratTo,
            //    Clarity = item.Clarity,
            //    Color = item.Color,
            //    Cut = item.Cut,
            //}).ToList();
            //var prices = listItem.Select(item => item.Price).ToList();
            //// Dispose the listItem to free up memory
            //listItem.Clear();
            //listItem = null;
            //var result = await _sender.Send(new CreateManyDiamondCriteriasCommand(mappedList, "1", false));
            //var getShapes = await _sender.Send(new GetAllDiamondShapeQuery());
            //var round = getShapes.FirstOrDefault(item => item.Shape.ToUpper() == "ROUND");
            //var pear = getShapes.FirstOrDefault(item => item.Shape.ToUpper() == "PEAR");
            //var mappedPriceList = result.Value.Select((item, index) => new DiamondPriceRequestDto(item.Id.Value, prices[index])).ToList();
            //var mappedPriceList2 = result.Value.Select((item, index) => new DiamondPriceRequestDto(item.Id.Value, prices[index])).ToList();

            //var result2 = await _sender.Send(new CreateManyDiamondPricesCommand(mappedPriceList, round.Id.Value, true, false));
            //var result3 = await _sender.Send(new CreateManyDiamondPricesCommand(mappedPriceList2, pear.Id.Value, true, false));

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
        //[ApiExplorerSettings(IgnoreApi = true)]
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
                var comando = new CreateDeliveryFeeCommand(city.Name, baseCost, city.Id);
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
            Order falseOrder = Order.Create(falseAccount.Id, PaymentType.Payall, PaymentMethod.ZALOPAY.Id, 100000m, 0, 10000m, "adfads");
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
            var cutEnums = Enum.GetValues(typeof(Cut));
            //Cut defaultCut = Cut.Ideal;
            decimal startPrice = 30_000;//vnd
            List<DiamondShape> getShapes = _dbContext.DiamondShapes.IgnoreQueryFilters().Where(x => x.Id != DiamondShape.ANY_SHAPES.Id).ToList();
            //_dbContext.DiamondShapes.IgnoreQueryFilters().ToList()
            //.Where(x => x.Id == DiamondShape.ROUND.Id
            //|| x.Id == DiamondShape.FANCY_SHAPES.Id)// only 2 shape
            //.ToList(); 
            List<(float caratFrom, float caratTo)> caratRange = new()
            {
                //new (0.01f, 0.03f),
                //new (0.03f, 0.07f),
                //new (0.07f, 0.14f),
                new (0.15f, 0.17f),
                new (0.18f, 0.22f),
                new (0.23f, 0.29f),
                new (0.30f, 0.39f),
                new (0.40f, 0.49f),
                new (0.50f, 0.69f),
                new (0.70f, 0.89f),
                new (0.90f, 0.99f),
                new (1.00f, 1.49f),
                new (1.50f, 1.99f),
            };
            var rowIncrementPrice = 10_000;
            var columnIncrementPrice = 5_000;
            var caratIncrementPrice = 10_000;
            // create round price
            var round = getShapes.FirstOrDefault(x => x.Id == DiamondShape.ROUND.Id);

            foreach (var carat in caratRange)
            {
                //List<DiamondCriteriaRequestDto> diamondCriteriaRequestDtos = new();
                var basePrice = (decimal)(carat.caratFrom * 100) * (startPrice * (decimal)(carat.caratTo * 100));
                var createresult = await _sender.Send(new CreateCriteriaFromRangeCommand(carat.caratFrom, carat.caratTo, round.Id.Value, false));
                var result = createresult.Value.First();
                //foreach (Cut cut in cutEnums)
                //{
                List<DiamondPriceRequestDto> prices = new();
                for (int i = 0; i < colorEnums.Length; i++)
                {
                    var color = (Color)colorEnums.GetValue(i);
                    var rowPrice = basePrice + rowIncrementPrice * i;
                    for (int j = 0; j < clarityEnums.Length; j++)
                    {
                        var clarity = (Clarity)clarityEnums.GetValue(j);
                        var columnPrice = rowPrice + columnIncrementPrice * j;
                        prices.Add(new DiamondPriceRequestDto(result.Id.Value, columnPrice,null, color, clarity));
                    }
                }

                var resultLab = await _sender.Send(new CreateManyDiamondPricesCommand(prices, round.Id.Value, true, false));
                var resultNatural = await _sender.Send(new CreateManyDiamondPricesCommand(prices, round.Id.Value, false, false));
                //}
            }
            // create fancy price
            //var allFancyShape = getShapes.Where(x => x.Id != DiamondShape.ROUND.Id && x.Id != DiamondShape.FANCY_SHAPES.Id).ToList();
            var allFancyShape = getShapes.Where(x => x.Id == DiamondShape.FANCY_SHAPES.Id).ToList();
            foreach (var carat in caratRange)
            {
                //List<DiamondCriteriaRequestDto> diamondCriteriaRequestDtos = new();
                var basePrice = (decimal)(carat.caratFrom * 100) * (startPrice * (decimal)(carat.caratTo * 100));
                foreach (var fancyShape in allFancyShape)
                {
                    List<DiamondPriceRequestDto> prices = new();

                    var createresult = await _sender.Send(new CreateCriteriaFromRangeCommand(carat.caratFrom, carat.caratTo, fancyShape.Id.Value, false));
                    var result = createresult.Value.First();
                    for (int i = 0; i < colorEnums.Length; i++)
                    {
                        var color = (Color)colorEnums.GetValue(i);
                        var rowPrice = basePrice + rowIncrementPrice * i;
                        for (int j = 0; j < clarityEnums.Length; j++)
                        {
                            var clarity = (Clarity)clarityEnums.GetValue(j);
                            var columnPrice = rowPrice + columnIncrementPrice * j;
                            prices.Add(new DiamondPriceRequestDto(result.Id.Value, columnPrice, null, color, clarity));
                        }
                    }
                    var resultLab = await _sender.Send(new CreateManyDiamondPricesCommand(prices, fancyShape.Id.Value, true, false));
                    var resultNatural = await _sender.Send(new CreateManyDiamondPricesCommand(prices, fancyShape.Id.Value, false, false));
                }
            }
            return Ok();
        }
        [HttpPost("seedsideDiamond")]
        public async Task<ActionResult> SeedCriteriaSideDiamond()
        {
            var colorEnums = Enum.GetValues(typeof(Color));
            var clarityEnums = Enum.GetValues(typeof(Clarity));
            Cut defaultCut = Cut.Excellent;
            decimal startPrice = 20_000;//vnd
            List<DiamondShape> getShapes = _dbContext.DiamondShapes.IgnoreQueryFilters().ToList(); //await _sender.Send(new GetAllDiamondShapeQuery());
            var getAnyShape = getShapes.FirstOrDefault(x => x.Id == DiamondShape.ANY_SHAPES.Id);
            List<(float caratFrom, float caratTo)> caratRange = new()
            {
                new (0.00f, 0.001f), // (0.00f, 0.001f
                new (0.001f, 0.01f),
                new (0.01f, 0.03f),
                new (0.03f, 0.07f),
                new (0.07f, 0.14f),
                new (0.14f, 0.18f),
                new (0.18f, 0.22f),
                new (0.22f, 0.29f),
                new (0.29f, 0.37f),
            };
            var rowIncrementPrice = 5_000;
            var columnIncrementPrice = 3_000;
            var caratIncrementPrice = 10_000;
            Stopwatch stopwatch = new();
            stopwatch.Start();
            foreach (var carat in caratRange)
            {
                List<DiamondPriceRequestDto> prices = new();
                var basePrice = Math.Clamp((decimal)(carat.caratFrom * 100) * (startPrice * (decimal)(carat.caratTo * 100)), startPrice, decimal.MaxValue);
                startPrice += caratIncrementPrice;
                var createresult = await _sender.Send(new CreateCriteriaFromRangeCommand(carat.caratFrom, carat.caratTo, getAnyShape.Id.Value, true));
                var result = createresult.Value.First();
                for (int i = 0; i < colorEnums.Length; i++)
                {
                    var color = (Color)colorEnums.GetValue(i);
                    var rowPrice = basePrice + rowIncrementPrice * i;
                    for (int j = 0; j < clarityEnums.Length; j++)
                    {
                        var clarity = (Clarity)clarityEnums.GetValue(j);
                        var columnPrice = rowPrice + columnIncrementPrice * j;
                        prices.Add(new DiamondPriceRequestDto(result.Id.Value, startPrice, null, color, clarity));
                    }
                }
                //var getAnyShape = getShapes.FirstOrDefault(x => x.Id == DiamondShape.ANY_SHAPES.Id);

                var resultAnyLab = await _sender.Send(new CreateManyDiamondPricesCommand(prices, getAnyShape.Id.Value, true, true));
                var resultAnyNatural = await _sender.Send(new CreateManyDiamondPricesCommand(prices, getAnyShape.Id.Value, false, true));
            }
            stopwatch.Stop();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Time to insert: " + stopwatch.ElapsedMilliseconds);
            Console.ResetColor();
            return Ok();
        }
        [HttpPost("seed/jewelry")]
        public async Task<ActionResult> SeedJewelry([FromQuery] SeedModelsCommand seedModelsCommand)
        {
            var createResult = await _sender.Send(seedModelsCommand);
            if (createResult.IsSuccess)
                return Ok();
            else
                return MatchError(createResult.Errors, ModelState);
        }
        [HttpGet("testpdf")]
        public async Task<ActionResult> TestPdf()
        {
            var pdf = _pdfService;
            var account = Account.Create(FullName.Create("a", "b"), "testing@gmail");
            var order = Order.Create(account.Id, PaymentType.Payall, PaymentMethodId.Parse("1"), 50_000_000, 0, 20_000, "abc", null, null, 40_000, 0, null, OrderId.Parse("1"));
            order.Items.Add(OrderItem.Create(order.Id, "", null, DiamondId.Parse("1"), 25_000_000, null, null, null, null, 0));
            order.Items.Add(OrderItem.Create(order.Id, "", JewelryId.Parse("2"), null, 25_000_000, null, null, null, null, 0));


            string result = pdf.GetTemplateHtmlStringFromOrder(order, account);
            //HttpContext.Response.Headers.ContentType = "text/html; charset=utf-8";
            return Ok(result);
        }
        [HttpGet("testpdf/download")]
        public async Task<ActionResult> TestPdfDownload()
        {
            var pdf = _pdfService;
            var account = Account.Create(FullName.Create("a", "b"), "testing@gmail");
            var order = Order.Create(account.Id, PaymentType.Payall, PaymentMethodId.Parse("1"), 50_000_000, 0, 20_000, "abc", null, null, 40_000, 0, null, OrderId.Parse("1"));
            var diamond = OrderItem.Create(order.Id, "", null, DiamondId.Parse("1"), 25_000_000, 20_000_000, null, null, null, 0);
            var jewellry = OrderItem.Create(order.Id, "", JewelryId.Parse("2"), null, 25_000_000, 20_000_000, null, null, null, 0);
            diamond.Diamond = Diamond.Create(DiamondShape.ROUND, new Diamond_4C(Cut.Excellent, Color.I, Clarity.FL, 0.22f, true), new Diamond_Details(Polish.Excellent, Symmetry.Excellent, Girdle.Thick, Fluorescence.Faint, Culet.Medium), new Diamond_Measurement(0.22f, 23f, 23f, "asdf"), 0.3m, "sfd");
            jewellry.Jewelry = Jewelry.Create(JewelryModelId.Create(), SizeId.Create(), MetalId.Create(), 2f, "sdf", DiamondShop.Domain.Common.Enums.ProductStatus.Active);
            order.Items.Add(diamond);

            order.Items.Add(jewellry);


            string result = pdf.GetTemplateHtmlStringFromOrder(order, account);
            var stream = pdf.ParseHtmlToPdf(result);
            //HttpContext.Response.Headers.ContentType = "text/html; charset=utf-8";
            return File(stream, "application/pdf", "billing_" + order.OrderCode + ".pdf");
        }
        [HttpGet("testinvoiceEmail")]
        public async Task<ActionResult> TestEmailInvoice()
        {
            var pdf = _pdfService;
            var account = Account.Create(FullName.Create("a", "b"), "testingwebandstuff@gmail.com");
            var order = Order.Create(account.Id, PaymentType.Payall, PaymentMethodId.Parse("1"), 50_000_000, 0, 20_000, "abc", null, null, 40_000, 20_000, null, OrderId.Parse("1"));
            order.Items.Add(OrderItem.Create(order.Id, "", null, DiamondId.Parse("1"), 25_000_000, 20_000_000, null, null, null, 0));
            order.Items.Add(OrderItem.Create(order.Id, "", JewelryId.Parse("2"), null, 25_000_000, 20_000_000, null, null, null, 0));

            await _emailService.SendInvoiceEmail(order, account);

            //HttpContext.Response.Headers.ContentType = "text/html; charset=utf-8";
            return Ok();
        }
        [HttpGet("testOrderPreparedEmail")]
        public async Task<ActionResult> TestOrderPreparedEmail()
        {
            var pdf = _pdfService;
            var account = Account.Create(FullName.Create("a", "b"), "testingwebandstuff@gmail.com");
            var order = Order.Create(account.Id, PaymentType.Payall, PaymentMethodId.Parse("1"), 50_000_000, 20_000, 0, "abc", null, null, 40_000, 20_000, DateTime.UtcNow, OrderId.Parse("1"));
            order.Items.Add(OrderItem.Create(order.Id, "", null, DiamondId.Parse("1"), 25_000_000, 20_000_000, null, null, null, 0));
            order.Items.Add(OrderItem.Create(order.Id, "", JewelryId.Parse("2"), null, 25_000_000, 20_000_000, null, null, null, 0));

            await _emailService.SendOrderPreparedEmail(order, account, DateTime.UtcNow);

            //HttpContext.Response.Headers.ContentType = "text/html; charset=utf-8";
            return Ok();
        }
        [HttpGet("testPdfFromSync")]
        public async Task<ActionResult> TestPdfFromSync()
        {
            var pdf = _pdfService;
            var account = Account.Create(FullName.Create("a", "b"), "testingwebandstuff@gmail.com");
            var order = Order.Create(account.Id, PaymentType.Payall, PaymentMethodId.Parse("1"), 50_000_000, 20_000, 0.312m, "abc", null, null, 40_000, 20_000, DateTime.UtcNow, OrderId.Parse("1"));
            var diamond = OrderItem.Create(order.Id, "", null, DiamondId.Parse("1"), 25_000_000, 20_000_000, null, null, null, 0);
            var jewellry = OrderItem.Create(order.Id, "", JewelryId.Parse("2"), null, 25_000_000, 20_000_000, null, null, null, 0);
            diamond.Diamond = Diamond.Create(DiamondShape.ROUND, new Diamond_4C(Cut.Excellent, Color.I, Clarity.FL, 0.22f, true), new Diamond_Details(Polish.Excellent, Symmetry.Excellent, Girdle.Thick, Fluorescence.Faint, Culet.Medium), new Diamond_Measurement(0.22f, 23f, 23f, "asdf"), 0.3m, "sfd");
            jewellry.Jewelry = Jewelry.Create(JewelryModelId.Create(), SizeId.Create(), MetalId.Create(), 2f, "sdf", DiamondShop.Domain.Common.Enums.ProductStatus.Active);
            order.Items.Add(diamond);

            order.Items.Add(jewellry);

            var stream = GeneratePdfService.CreatePdfInvoice(order, account, new LocationOptions() { ShopOrignalLocation = new DiamondShop.Domain.BusinessRules.LocationRules() });
            //HttpContext.Response.Headers.ContentType = "text/html; charset=utf-8";
            return File(stream, "application/pdf", "test" + order.OrderCode + ".pdf");
        }
    }
}
