using DiamondShop.Application.Dtos.Requests.Diamonds;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.DiamondCriterias.Commands.CreateFromRange;
using DiamondShop.Application.Usecases.DiamondPrices.Commands.CreateMany;
using DiamondShop.Application.Usecases.Diamonds.Commands.Create;
using DiamondShop.Application.Usecases.Jewelries.Commands.Seeding;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories.DeliveryRepo;
using DiamondShop.Infrastructure.Databases;
using DiamondShop.Infrastructure.Options;
using DiamondShopSystem.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenQA.Selenium.DevTools.V127.CSS;
using System.Diagnostics;

namespace DiamondShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("SEED DATA")]
    public class SeedDataController : ApiControllerBase
    {
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

        public SeedDataController(ILogger<WeatherForecastController> logger, IDateTimeProvider dateTimeProvider, IBlobFileServices blobFileServices, IOptions<PaypalOption> paypal, IOptions<VnpayOption> vnpayOption, IHttpContextAccessor httpContextAccessor, ISender sender, IOptions<LocationOptions> locationOptions, DiamondShopDbContext dbContext, ILocationService locationService, IDeliveryFeeRepository deliveryFeeRepository, IEmailService emailService, IPaymentService paymentService, IPdfService pdfService)
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
        [HttpPost("seed-price-main-diamond")]
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
                        prices.Add(new DiamondPriceRequestDto(result.Id.Value, columnPrice, null, color, clarity));
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
        [HttpPost("seed-price-side-diamond")]
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
        [HttpPost("seed-main-diamond")]
        public async Task<ActionResult> SeedDiamondBellow1Carat(SeedMainDiamondRequest request)
        {
            var allShape = _dbContext.DiamondShapes.Where(x => x.Id != DiamondShape.ANY_SHAPES.Id && x.Id != DiamondShape.FANCY_SHAPES.Id).ToList();
            if (allShape.Count != 10)
                throw new Exception();
            var parsedShapeIds = request.shapeIds.Select(x => DiamondShapeId.Parse(x)).ToList();
            var selectedShape = allShape.Where(x => parsedShapeIds.Contains(x.Id)).ToList();
            var cutEnums = CutHelper.GetCutList().Where(x => x >= request.cutFrom && x <= request.cutTo).ToArray();
            var colorEnums = ColorHelper.GetColorList().Where(x => x >= request.colorFrom && x <= request.colorTo).ToArray();
            var clarityEnums = ClarityHelper.GetClarityList().Where(x => x >= request.clarityFom && x <= request.clarityTo).ToArray();
            if (selectedShape.Count == 0 || colorEnums.Count() == 0 || clarityEnums.Count() == 0)
                throw new Exception();
            //var detail = new Diamond_Details();
           // var measurement = new Diamond_Measurement();
            //new CreateDiamondCommand();
            throw new Exception();

        }
        public T RandomEnum<T>(T[] values)
        {
            //T[] values = (T[])Enum.GetValues(typeof(T));
            return values[new Random().Next(0, values.Length)];
        }
    }
    public record SeedMainDiamondRequest(float caratFrom, float caratTo, Cut? cutFrom, Cut? cutTo, Color colorFrom, Color colorTo, Clarity clarityFom, Clarity clarityTo, bool isLabGrown, string[] shapeIds);
    
}
