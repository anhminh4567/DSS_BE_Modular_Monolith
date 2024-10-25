using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Infrastructure.Services.Excels;
using FluentEmail.Core;
using HtmlAgilityPack;
using Syncfusion.Compression.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DiamondShop.Infrastructure.Services.Scrapers
{
    public class CaohungDiamondPriceScraper
    {
        // user agent is for some website that prevent scrapping
        private const string UserAgent ="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";
        private static string[] ListUrls = { "https://www.stonealgo.com/lab-grown-diamond-prices/?i=round" };
        public void ExecuteCaohungScraper()
        {
            var url = "https://caohungdiamond.com/bang-gia-kim-cuong/";
            var web = new HtmlWeb();
            web.UserAgent = UserAgent;
            var document = web.Load(url);
            var root = document.DocumentNode;
            var getAllTablePrice = root.SelectNodes("//div[@class='price-content']");
            List<CaohungDiamondPrice> caohungPrices = new();
            foreach(var priceTable in getAllTablePrice)
            {
                var getTitle = priceTable.SelectSingleNode(".//h3").SelectNodes(".//span").Last().InnerText.Trim();
                var getMilimiter = decimal.Parse(getTitle.Replace("ly",".")); //.Replace("&lt;1CT", "").Replace("&gt;1CT", "").Replace("&lt;3CT", "")
                var caratRange = FromMilToCaratRange(getMilimiter);
                var tableRows = priceTable.SelectSingleNode(".//table").SelectNodes(".//tr");
                var firstRow = tableRows[0];
                var clarities = firstRow.SelectNodes(".//th").Skip(1).Select(node => FromCaohungToClarity( node.InnerText.Trim() )).ToList();
                var colors = tableRows.Skip(1).Select(node => FromCaohungToColor( node.SelectSingleNode(".//td").InnerText.Trim())).ToList();
                for (int i = 0; i < tableRows.Count; i++)
                {
                    if (i == 0)
                        continue;
                    var row = tableRows[i];
                    var datas = row.SelectNodes(".//td").Skip(1).ToList();
                    var color = FromCaohungToColor(row.SelectSingleNode(".//td").InnerText.Trim().ToUpper());
                    for (int j = 0; j < datas.Count; j++)
                    {
                        var price = FromCaohungPriceToPrice( datas[j].InnerText.Trim());
                        var clarity = clarities[j];
                        //var color = colors[i];
                        CaohungDiamondPrice priceObj = new CaohungDiamondPrice()
                        {
                            CaratFrom = caratRange.Value.caratFrom,
                            CaratTo = caratRange.Value.caratTo,
                            Clarity = clarity,
                            Color = color,
                            Cut = Cut.Excelent,
                            Price = price,
                        };
                        caohungPrices.Add(priceObj);
                    }
                }
            }
            var count = caohungPrices.Count;
            var excelWriter = new ExcelSyncfunctionService();
            var getWorkSheet = ExcelSyncfunctionService.GetExcelApplication().CreateWorkBook().Worksheets.First();
            for (int i = 0; i < caohungPrices.Count; i++)
            {
                getWorkSheet.WriteLine(caohungPrices[i],i,0);
            }
            string fileName = "diamondPrice.xlsx";
            string path = Directory.GetCurrentDirectory();
            string fullPath = Path.Combine(path, fileName);
            FileStream file = new FileStream(fullPath, FileMode.Create, FileAccess.ReadWrite);
            try
            {
                getWorkSheet.Workbook.SaveAs(file);
            }
            catch (Exception ex) 
            {
            }
            finally
            {
                file.Dispose();
            }
        }

        private decimal FromCaohungPriceToPrice(string price)
        {
            return decimal.Parse( price.Replace(",", "")); 
        }
        private Clarity FromCaohungToClarity(string clarity)
        {
            Clarity? result = clarity switch
            {
                "IF" =>  Clarity.IF,
                "VVS1" => Clarity.VVS1,
                "VVS2" => Clarity.VVS2,
                "VS1" => Clarity.VS1 ,
                "VS2" => Clarity.VS2,
                _ => throw new Exception(),
            };
            return result.Value;
        }
        private Color FromCaohungToColor(string color)
        {
            Color? result = color switch
            {
                "D" => Color.D,
                "E" => Color.E,
                "F" => Color.F,
                "J" => Color.J,
                _ => throw new Exception(),
            };
            return result.Value;
        }
        private (decimal caratFrom, decimal caratTo)? FromMilToCaratRange(decimal milimeter)
        {
            // from cao hung https://caohungdiamond.com/bang-quy-doi-kich-thuoc-kim-cuong/
            (decimal caratFrom, decimal caratTo)? result = milimeter switch
            {
                >= 0.8m and < 1m => (0.0025m, 0.005m),
                >= 1m and < 1.1m => (0.005m, 0.0067m),
                >= 1.1m and < 1.2m => (0.0067m, 0.009m),
                >= 1.2m and < 1.25m => (0.009m, 0.01m),
                >= 1.25m and < 1.3m => (0.01m, 0.01m),
                >= 1.3m and < 1.5m => (0.01m, 0.015m),
                >= 1.5m and < 1.75m => (0.015m, 0.02m),
                >= 1.75m and < 1.8m => (0.02m, 0.03m),
                >= 1.8m and < 2m => (0.03m, 0.04m),
                >= 2m and < 2.2m => (0.04m, 0.06m),
                >= 2.2m and < 2.5m => (0.06m, 0.08m),
                >= 2.5m and < 2.75m => (0.08m, 0.08m),
                >= 2.75m and < 3m => (0.08m, 0.19m),
                >= 3m and < 3.25m => (0.19m, 0.14m),
                >= 3.25m and < 3.5m => (0.14m, 0.17m),
                >= 3.5m and < 3.75m => (0.17m, 0.21m),
                >= 3.75m and < 4m => (0.21m, 0.25m),
                >= 4m and < 4.25m => (0.25m, 0.28m),
                >= 4.25m and < 4.5m => (0.28m, 0.36m),
                >= 4.5m and < 4.75m => (0.36m, 0.44m),
                >= 4.75m and < 5m => (0.44m, 0.5m),
                >= 5m and < 5.25m => (0.5m, 0.56m),
                >= 5.25m and < 5.5m => (0.56m, 0.68m),
                >= 5.5m and < 5.75m => (0.68m, 0.75m),
                >= 5.75m and < 6m => (0.75m, 0.84m),
                >= 6m and < 6.25m => (0.84m, 0.93m),
                >= 6.25m and < 6.5m => (0.93m, 1m),
                >= 6.5m and < 6.8m => (1m, 1.25m),
                >= 6.8m and < 7m => (1.25m, 1.39m),
                >= 7m and < 7.3m => (1.39m, 1.5m),
                >= 7.3m and < 7.5m => (1.5m, 1.67m),
                >= 7.5m and < 7.75m => (1.67m, 1.75m),
                >= 7.75m and < 8m => (1.75m, 2m),
                >= 8m and < 8.25m => (2m, 2.11m),
                >= 8.25m and < 8.5m => (2.11m, 2.43m),
                >= 8.5m and < 8.7m => (2.43m, 2.5m),
                >= 8.7m and < 9m => (2.5m, 2.75m),
                >= 9m and < 9.1m => (2.75m, 3m),
                >= 9.1m and < 9.5m => (3m, 3.35m),
                >= 9.5m and < 9.75m => (3.35m, 3.61m),
                >= 9.75m and < 10m => (3.61m, 3.87m),
                >= 10m and < 10.25m => (3.87m, 4.16m),
                _ => null, //throw new ArgumentOutOfRangeException(nameof(milimeter), "Value not supported"),

            };
            return result;
        }
        private  Cut? ApplyCorrectCutFromAlgoStone(double score)
        {
            Cut? correctCutFromScore = score switch
            {
                >= 3.0 and < 4.0  =>  Cut.Good ,
                >= 4.0 and < 6.0 => Cut.Very_Good,
                >= 6.0 and < 8.0 => Cut.Excelent,
                _ => null
            };
            return correctCutFromScore;
        }
        //10's are totally perfect (they don't exist - they would require extremely precise measurements that we can't get from a GIA certificate)
        //9's are the very best Excellent cut diamonds
        //8's are above average for Excellent cut diamonds
        //7's are average for Excellent cut diamonds
        //6's are below average for Excellent cut diamonds, and above average for Very Good cut diamonds
        //5's are average for Very Good cut diamonds
        //4's are below average for Very Good cut diamonds, and above average for Good cut diamonds
        //3's are average for Good cut diamonds
        //2's are below average for Good cut diamonds, and above average for Fair cut diamonds
        //1's are average Fair cut diamonds
        //0's are below average Fair cut diamonds
    }
    public class CaohungDiamondPrice
    {
        public Cut Cut { get; set; }
        public Clarity Clarity { get; set; }
        public Color Color { get; set; }
        public decimal CaratFrom { get; set; }
        public decimal CaratTo { get; set; }
        public decimal Price { get; set; }
    }
}
