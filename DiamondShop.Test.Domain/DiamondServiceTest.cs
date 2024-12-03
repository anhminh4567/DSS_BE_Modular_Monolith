using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Services.Implementations;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Test.Domain
{
    public class DiamondServiceTest
    {
        private List<DiamondPrice> _diamondPrices;
        private Color colorFrom = Color.E;
        private Color colorTo = Color.D;
        private Clarity clarityFrom = Clarity.VVS2;
        private Clarity clarityTo = Clarity.VVS1;
        private JewelryId jewelryId = JewelryId.Parse("1");
        private DiamondRule BaseDiamondRule = new();
        public DiamondServiceTest()
        {
            bool Islab = true;
            bool IsSide = true;
            DiamondShape currentShape = DiamondShape.FANCY_SHAPES;
            DiamondCriteria criteria1 = DiamondCriteria.CreateSideDiamondCriteria(0.01f, 0.05f,  currentShape);
            DiamondCriteria criteria2 = DiamondCriteria.CreateSideDiamondCriteria(0.01f, 0.05f, currentShape);
            DiamondCriteria criteria3 = DiamondCriteria.CreateSideDiamondCriteria(0.01f, 0.05f, currentShape);
            DiamondCriteria criteria4 = DiamondCriteria.CreateSideDiamondCriteria(0.01f, 0.05f, currentShape);

            _diamondPrices = new List<DiamondPrice>();
            var price1 = DiamondPrice.CreateSideDiamondPrice(criteria1.Id, 500_000m, Islab, currentShape, null, colorFrom, clarityFrom); price1.Criteria = criteria1;
            var price2 = DiamondPrice.CreateSideDiamondPrice(criteria2.Id, 600_000m, Islab, currentShape, null, colorTo, clarityFrom); price2.Criteria = criteria2;
            var price3 = DiamondPrice.CreateSideDiamondPrice(criteria3.Id, 700_000m, Islab, currentShape, null, colorFrom, clarityTo); price3.Criteria = criteria3;
            var price4 = DiamondPrice.CreateSideDiamondPrice(criteria4.Id, 800_000m, Islab, currentShape, null, colorTo, clarityTo); price4.Criteria = criteria4;
            _diamondPrices.Add(price1);
            _diamondPrices.Add(price2);
            _diamondPrices.Add(price3);
            _diamondPrices.Add(price4);
        }

        [Fact]
        public async Task SideDiamondPrice_AllPriceFound_Should_AverageAll()
        {
            //arrange
            var sideDiamond = JewelrySideDiamond.Create(jewelryId, 0.2f, 10, colorFrom, colorTo, clarityFrom, clarityTo, SettingType.Flush);
            sideDiamond.DiamondShape = DiamondShape.FANCY_SHAPES;
            sideDiamond.DiamondShapeId = sideDiamond.DiamondShape.Id;
            var expectedPriceFound = 4;
            var exptectAveragePrice = _diamondPrices.Average(x => x.Price);
            var exptectedTotalPrice = exptectAveragePrice * sideDiamond.Quantity;
            //act
            await DiamondServices.GetSideDiamondPriceGlobal(sideDiamond, _diamondPrices, BaseDiamondRule);

            //assert
            sideDiamond.TotalPriceMatched.Should().Be(expectedPriceFound);
            sideDiamond.AveragePricePerCarat.Should().Be(exptectAveragePrice);
            sideDiamond.TotalPrice.Should().Be(exptectedTotalPrice);
        }
        [Fact]
        public async Task SideDiamondPrice_SomeFound_Should_Average_SomeOnly()
        {
            //arrange
            string jsonStringOldPrice = JsonConvert.SerializeObject(_diamondPrices);
            var listRemoveONEprice = JsonConvert.DeserializeObject<List<DiamondPrice>>(jsonStringOldPrice);
            listRemoveONEprice!.RemoveAt(listRemoveONEprice.Count - 1);//remove last
            var sideDiamond = JewelrySideDiamond.Create(jewelryId, 0.2f, 10, colorFrom, colorTo, clarityFrom, clarityTo, SettingType.Flush);
            sideDiamond.DiamondShape = DiamondShape.FANCY_SHAPES;
            sideDiamond.DiamondShapeId = sideDiamond.DiamondShape.Id;
            
            var expectedPriceFound = listRemoveONEprice.Count;
            var exptectAveragePrice = listRemoveONEprice.Average(x => x.Price);
            var exptectedTotalPrice = exptectAveragePrice * sideDiamond.Quantity;
            //act
            await DiamondServices.GetSideDiamondPriceGlobal(sideDiamond, listRemoveONEprice, BaseDiamondRule);

            //assert
            sideDiamond.TotalPriceMatched.Should().Be(expectedPriceFound);
            sideDiamond.AveragePricePerCarat.Should().Be(exptectAveragePrice);
            sideDiamond.TotalPrice.Should().Be(exptectedTotalPrice);
        }
        [Fact]
        public async Task SideDiamondPrice_ExcludePriceNotInRange_Should_Average_AllFound()
        {
            //arrange
            string jsonStringOldPrice = JsonConvert.SerializeObject(_diamondPrices);
            var listAddOnePriceOutOfRange = JsonConvert.DeserializeObject<List<DiamondPrice>>(jsonStringOldPrice);
            DiamondCriteria outOfRangeCriteria = DiamondCriteria.CreateSideDiamondCriteria(0.01f, 0.05f, DiamondShape.ANY_SHAPES);
            //the criteria is off by the color, none of the side or price is in F, they are in d and e
            DiamondPrice newPriceOutOrRange = DiamondPrice.CreateSideDiamondPrice(outOfRangeCriteria.Id,800_000m,true,DiamondShape.FANCY_SHAPES,null,Color.D,Clarity.FL);
            newPriceOutOrRange.Criteria = outOfRangeCriteria;
            listAddOnePriceOutOfRange.Add(newPriceOutOrRange);

            var sideDiamond = JewelrySideDiamond.Create(jewelryId, 0.2f, 10, colorFrom, colorTo, clarityFrom, clarityTo, SettingType.Flush);
            sideDiamond.DiamondShape = DiamondShape.FANCY_SHAPES;
            sideDiamond.DiamondShapeId = sideDiamond.DiamondShape.Id;

            var expectedPriceFound = listAddOnePriceOutOfRange.Count - 1;
            var exptectAveragePrice = _diamondPrices.Average(x => x.Price);
            var exptectedTotalPrice = exptectAveragePrice * sideDiamond.Quantity;
            //act
            await DiamondServices.GetSideDiamondPriceGlobal(sideDiamond, listAddOnePriceOutOfRange, BaseDiamondRule);

            //assert
            sideDiamond.TotalPriceMatched.Should().Be(expectedPriceFound);
            sideDiamond.AveragePricePerCarat.Should().Be(exptectAveragePrice);
            sideDiamond.TotalPrice.Should().Be(exptectedTotalPrice);
        }
        [Fact]
        public async Task SideDiamondPrice_NoPriceFound_Should_Return_UnknownPrice()
        {
            //arrange
            
            var sideDiamond = JewelrySideDiamond.Create(jewelryId, 0.2f, 10, colorFrom, colorTo, clarityFrom, clarityTo, SettingType.Flush);
            sideDiamond.DiamondShape = DiamondShape.FANCY_SHAPES;
            sideDiamond.DiamondShapeId = sideDiamond.DiamondShape.Id;

            var expectedPriceFound = 1;
            var exptectAveragePrice = 0;
            var exptectedTotalPrice = 0;
            var expectedUnknownPrice = DiamondPrice.CreateUnknownSideDiamondPrice(true);
            //act
            await DiamondServices.GetSideDiamondPriceGlobal(sideDiamond, new List<DiamondPrice>(), BaseDiamondRule);

            //assert
            sideDiamond.TotalPriceMatched.Should().Be(expectedPriceFound);
            sideDiamond.AveragePricePerCarat.Should().Be(exptectAveragePrice);
            sideDiamond.TotalPrice.Should().Be(exptectedTotalPrice);
            sideDiamond.DiamondPrice.Count.Should().Be(1);
            sideDiamond.DiamondPrice.First().ForUnknownPrice.Should().NotBeNullOrEmpty();
        }
        [Fact]
        public async Task SideDiamondPrice_PricefoundButtooSmall_Should_Return_MinSideDiamondAveragePriceFromRule()
        {
            //arrange

            var sideDiamond = JewelrySideDiamond.Create(jewelryId, 0.2f, 10, colorFrom, colorTo, clarityFrom, clarityTo, SettingType.Flush);
            sideDiamond.DiamondShape = DiamondShape.FANCY_SHAPES;
            sideDiamond.DiamondShapeId = sideDiamond.DiamondShape.Id;

            var criteria = DiamondCriteria.CreateSideDiamondCriteria(0.001f, sideDiamond.AverageCarat, sideDiamond.DiamondShape);
            var sideDiamondPrice = DiamondPrice.CreateSideDiamondPrice(criteria.Id, 1000m, true, sideDiamond.DiamondShape,null,sideDiamond.ColorMin,sideDiamond.ClarityMin);
            sideDiamondPrice.Criteria = criteria;

            var expectedPriceFound = 1;
            var exptectAveragePriceWithoutRule = sideDiamond.Quantity * sideDiamondPrice.Price;
            var exptectAveragePriceWithRule = Math.Clamp(exptectAveragePriceWithoutRule, BaseDiamondRule.MinimalSideDiamondAveragePrice,decimal.MaxValue);

            //act
            await DiamondServices.GetSideDiamondPriceGlobal(sideDiamond, new List<DiamondPrice>() { sideDiamondPrice }, BaseDiamondRule);

            //assert
            sideDiamond.TotalPriceMatched.Should().Be(1);
            sideDiamond.AveragePricePerCarat.Should().Be(sideDiamondPrice.Price);
            (sideDiamond.Quantity * sideDiamond.DiamondPrice.Sum(x => x.Price)).Should().Be(exptectAveragePriceWithoutRule);//this is the price without rule
            sideDiamond.TotalPrice.Should().Be(exptectAveragePriceWithRule);
        }
        [Fact]
        public async Task MainPrice_PriceFound_Truepricetoosmall_Trueprice_Should_MinpriceRule()
        {
            //arrange
            var diamondRule = new DiamondRule();
            var diamond4c = new Diamond_4C(Cut.Very_Good, Color.I, Clarity.VVS1, 0.15f, true);
            Diamond mainDiamond = Diamond.Create(DiamondShape.ROUND, diamond4c, new Diamond_Details(Polish.Good, Symmetry.Good, Girdle.Medium, Fluorescence.Medium, Culet.Medium), new Diamond_Measurement(2f, 22f, 2f, "whatever 2"), 1, "asdfasdf");
            mainDiamond.DiamondShape = DiamondShape.ROUND;

            DiamondCriteria mainCriteria = DiamondCriteria.Create(diamond4c.Carat -0.1f,diamond4c.Carat+ 0.1f,DiamondShape.ROUND);

            DiamondPrice mainPrice = DiamondPrice.Create(DiamondShape.ROUND.Id, mainCriteria.Id, 20000, diamond4c.isLabDiamond, diamond4c.Cut, diamond4c.Color, diamond4c.Clarity);
            mainPrice.Criteria = mainCriteria;

            var expectedPriceWithoutRules = mainPrice.Price * (decimal) mainDiamond.Carat;
            var eppectedPriceWithRules = Math.Clamp(expectedPriceWithoutRules, diamondRule.MinimalMainDiamondPrice, decimal.MaxValue);
            //act
            await DiamondServices.GetDiamondPriceGlobal(mainDiamond, new List<DiamondPrice>() { mainPrice }, diamondRule);

            //assert
            mainDiamond.DiamondPrice.Should().Be(mainPrice);
            (mainDiamond.DiamondPrice.Price * (decimal)mainDiamond.Carat).Should().Be(expectedPriceWithoutRules);
            mainDiamond.TruePrice.Should().Be(eppectedPriceWithRules);
        }
    }
}
