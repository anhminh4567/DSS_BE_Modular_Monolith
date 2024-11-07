using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
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
        public DiamondServiceTest()
        {
            bool Islab = true;
            bool IsSide = true;
            DiamondShape currentShape = DiamondShape.FANCY_SHAPES;
            DiamondCriteria criteria1 = DiamondCriteria.CreateSideDiamondCriteria(0.01f, 0.05f, Clarity.VVS2, Color.E);
            DiamondCriteria criteria2 = DiamondCriteria.CreateSideDiamondCriteria(0.01f, 0.05f, Clarity.VVS1, Color.E);
            DiamondCriteria criteria3 = DiamondCriteria.CreateSideDiamondCriteria(0.01f, 0.05f, Clarity.VVS2, Color.D);
            DiamondCriteria criteria4 = DiamondCriteria.CreateSideDiamondCriteria(0.01f, 0.05f, Clarity.VVS1, Color.D);

            _diamondPrices = new List<DiamondPrice>();
            var price1 = DiamondPrice.CreateSideDiamondPrice(criteria1.Id, 500_000m, Islab, currentShape); price1.Criteria = criteria1;
            var price2 = DiamondPrice.CreateSideDiamondPrice(criteria2.Id, 600_000m, Islab, currentShape); price2.Criteria = criteria2;
            var price3 = DiamondPrice.CreateSideDiamondPrice(criteria3.Id, 700_000m, Islab, currentShape); price3.Criteria = criteria3;
            var price4 = DiamondPrice.CreateSideDiamondPrice(criteria4.Id, 800_000m, Islab, currentShape); price4.Criteria = criteria4;
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
            await DiamondServices.GetSideDiamondPriceGlobal(sideDiamond, _diamondPrices);

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
            await DiamondServices.GetSideDiamondPriceGlobal(sideDiamond, listRemoveONEprice);

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
            DiamondCriteria outOfRangeCriteria = DiamondCriteria.CreateSideDiamondCriteria(0.01f, 0.05f, Clarity.VVS1, Color.F);
            //the criteria is off by the color, none of the side or price is in F, they are in d and e
            DiamondPrice newPriceOutOrRange = DiamondPrice.CreateSideDiamondPrice(outOfRangeCriteria.Id,800_000m,true,DiamondShape.FANCY_SHAPES);
            newPriceOutOrRange.Criteria = outOfRangeCriteria;
            listAddOnePriceOutOfRange.Add(newPriceOutOrRange);

            var sideDiamond = JewelrySideDiamond.Create(jewelryId, 0.2f, 10, colorFrom, colorTo, clarityFrom, clarityTo, SettingType.Flush);
            sideDiamond.DiamondShape = DiamondShape.FANCY_SHAPES;
            sideDiamond.DiamondShapeId = sideDiamond.DiamondShape.Id;

            var expectedPriceFound = listAddOnePriceOutOfRange.Count - 1;
            var exptectAveragePrice = _diamondPrices.Average(x => x.Price);
            var exptectedTotalPrice = exptectAveragePrice * sideDiamond.Quantity;
            //act
            await DiamondServices.GetSideDiamondPriceGlobal(sideDiamond, listAddOnePriceOutOfRange);

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
            await DiamondServices.GetSideDiamondPriceGlobal(sideDiamond, new List<DiamondPrice>());

            //assert
            sideDiamond.TotalPriceMatched.Should().Be(expectedPriceFound);
            sideDiamond.AveragePricePerCarat.Should().Be(exptectAveragePrice);
            sideDiamond.TotalPrice.Should().Be(exptectedTotalPrice);
            sideDiamond.DiamondPrice.Count.Should().Be(1);
            sideDiamond.DiamondPrice.First().ForUnknownPrice.Should().NotBeNullOrEmpty();
        }
    }
}
