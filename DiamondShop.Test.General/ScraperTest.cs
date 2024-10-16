using DiamondShop.Infrastructure.Services.Scrapers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Test.General
{
    public class ScraperTest
    {
        //[Fact(Skip = "not now")]
        [Fact(Skip = " not now")]
        public void TestScraper()
        {
            CaohungDiamondPriceScraper scraper = new CaohungDiamondPriceScraper(); 
            scraper.ExecuteCaohungScraper();
        }
        [Fact(Skip = "not now")]
        public void TestSelenium()
        {
            AlgostoneDiamondPriceScraper scraper = new AlgostoneDiamondPriceScraper();
            scraper.ExecuteAlgoStoneScraper();
        }
        [Fact()]
        public void TestGetDistant()
        {
            DistanceScraper scraper = new DistanceScraper();
            scraper.Execute();
        }
    }
}
