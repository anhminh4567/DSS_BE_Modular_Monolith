﻿using DiamondShop.Infrastructure.Services.Scrapers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Test.General
{
    public class ScraperTest
    {
        [Fact(Skip = "not now")]
        public void TestScraper()
        {
            CaohungDiamondPriceScraper scraper = new CaohungDiamondPriceScraper(); 
            scraper.ExecuteCaohungScraper();
        }
        [Fact]
        public void TestSelenium()
        {
            AlgostoneDiamondPriceScraper scraper = new AlgostoneDiamondPriceScraper();
            scraper.ExecuteAlgoStoneScraper();
        }
    }
}
