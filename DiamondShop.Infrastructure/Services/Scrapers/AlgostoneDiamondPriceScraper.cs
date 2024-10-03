using DiamondShop.Domain.Models.Diamonds.Enums;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Scrapers
{
    public class AlgostoneDiamondPriceScraper
    {
        private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";
        private static string[] ListUrls = { "https://www.stonealgo.com/lab-grown-diamond-prices/?i=round" };

        private const string RoundDiamondUrlsFromTo = "https://www.stonealgo.com/diamond-search/s/nt1f833ca334ed97318d26beffaf881128d";
        public void ExecuteAlgoStoneScraper()
        {
            //var excelWriter = new ExcelSyncfunctionService();
            //var web = new HtmlWeb();
            //web.UserAgent = UserAgent;
            //var document = web.Load(ListUrls[0]);
            ////Console.WriteLine(document.ParsedText);
            //var root = document.DocumentNode;
            //var tableRowsDiamondsNodes = root.SelectNodes("//tr[@data-table_link]");
            //foreach (var node in tableRowsDiamondsNodes)
            //{
            //    var innerFirstLink = node.SelectSingleNode(".//a[@href]");
            //    var title = innerFirstLink.SelectSingleNode(".//span").InnerText.Trim();
            //    var caratAmount = decimal.Parse((title.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[0]));
            //    int positionNo6 = 5;
            //    var priceRange = node.SelectNodes("//.td")[positionNo6];
            //}
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            var driver = new ChromeDriver(options);
            try
            {
                driver.Navigate().GoToUrl(RoundDiamondUrlsFromTo);
                var title = driver.FindElement(By.Id("sa-res_2_header_holder"));
                var diamondBoard = driver.FindElement(By.Id("sa-res_2"));
                var diamondRows = diamondBoard.FindElements(By.XPath("./div"));
                var count = diamondRows.Count;
                for (int i = 0; i < count; i++)
                {
                    var row = diamondRows[i];
                    var dataPart = row.FindElement(By.XPath(".//div[contains(@class, 'product-row-text-col')]")).FindElement(By.XPath("./div"));
                    var datas = dataPart.FindElements(By.XPath("./div"));
                    var cutScore = datas[0].FindElement(By.TagName("button")).Text;
                    var carat = datas[1].FindElement(By.ClassName("text-gray-700")).Text;
                    var clarity = datas[2].FindElement(By.ClassName("text-gray-700")).Text;
                    var color = datas[3].FindElement(By.ClassName("text-gray-700")).Text;
                    var price = datas[7].FindElement(By.ClassName("text-gray-700")).Text;//.text-gray-700.font-bold
                    //Debug.WriteLine(dataPart.GetAttribute("innerHTML"));
                }
            }
            catch(Exception ex) 
            {
            }
            finally{
                driver.Quit();
            }

 
        }
        private static string GetHtml()
        {
            return "";
        }
        private Cut? ApplyCorrectCutFromAlgoStone(double score)
        {
            Cut? correctCutFromScore = score switch
            {
                >= 3.0 and < 4.0 => Cut.Good,
                >= 4.0 and < 6.0 => Cut.Very_Good,
                >= 6.0 and < 8.0 => Cut.Ideal,
                >= 8.0 and <= 10.0 => Cut.Astor_Ideal,
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
}

