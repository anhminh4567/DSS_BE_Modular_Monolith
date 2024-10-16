using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Scrapers
{
    internal class DistanceScraper
    {
        private const string URL = "https://travelcar.vn/blog/khoang-cach-giua-cac-tinh-thanh-pho-tren-ca-nuoc-10446.html";
        private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";
        public void Execute()
        {
            //var web = new HtmlWeb();
            //web.UserAgent = UserAgent;
            //var document = web.Load(URL);
            //var root = document.DocumentNode;
            //var getAllTablePrice = root.SelectNodes("//table[@style='text-align:center; width:100%']");
            //var getTableHCM = getAllTablePrice[1];
            //var allRow = getTableHCM.SelectNodes(".//tr").Skip(1).ToList();
            //foreach(var row in allRow)
            //{
            //    // &nbsp;
            //    var datas = row.SelectNodes(".//td");
            //    var firstCity = datas[0].InnerText;
            //    var firstDistanceKm = datas[1].InnerText;
            //    string secondCity;
            //    string secondDistanceKm;
            //    if (datas[2].InnerText.Trim() != "&nbsp")
            //        secondCity = datas[2].InnerText.Trim();
            //    if (datas[3].InnerText.Trim() != "&nbsp")
            //        secondDistanceKm = datas[3].InnerText.Trim();

            //    Debug.WriteLine(row.InnerHtml);
            //}
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            var driver = new ChromeDriver(options);
            try
            {
                driver.Navigate().GoToUrl(URL);
                //var title = driver.FindElement(By.Id("sa-res_2_header_holder"));
                //var diamondBoard = driver.FindElement(By.Id("sa-res_2"));
                //var diamondRows = diamondBoard.FindElements(By.XPath("./div"));

                var getAllTablePrice = driver.FindElements(By.XPath("//table[@style='text-align:center; width:100%']"));
                var getTableHCM = getAllTablePrice[1];
                var allRow = getTableHCM.FindElements(By.XPath(".//tr")).Skip(1).ToList();
                foreach (var row in allRow)
                {
                    // &nbsp;
                    var datas = row.FindElements(By.XPath(".//td"));
                    var firstCity = datas[0].Text.Trim();
                    var firstDistanceKm = datas[1].Text.Trim();
                    string secondCity;
                    string secondDistanceKm;
                    if (string.IsNullOrEmpty(datas[2].Text.Trim()))
                        secondCity = datas[2].Text.Trim();
                    if (string.IsNullOrEmpty(datas[3].Text.Trim()))
                        secondDistanceKm = datas[3].Text.Trim();
                    Debug.WriteLine(row.GetAttribute("innerHTML"));
                }
                //for (int i = 0; i < count; i++)
                //{
                //    var row = diamondRows[i];
                //    var dataPart = row.FindElement(By.XPath(".//div[contains(@class, 'product-row-text-col')]")).FindElement(By.XPath("./div"));
                //    var datas = dataPart.FindElements(By.XPath("./div"));
                //    var cutScore = datas[0].FindElement(By.TagName("button")).Text;
                //    var carat = datas[1].FindElement(By.ClassName("text-gray-700")).Text;
                //    var clarity = datas[2].FindElement(By.ClassName("text-gray-700")).Text;
                //    var color = datas[3].FindElement(By.ClassName("text-gray-700")).Text;
                //    var price = datas[7].FindElement(By.ClassName("text-gray-700")).Text;//.text-gray-700.font-bold
                //    //Debug.WriteLine(dataPart.GetAttribute("innerHTML"));
                //}
            }
            catch (Exception ex)
            {
            }
            finally
            {
                driver.Quit();
            }
        }
    }
    public record DistantFromHCMDto
    {
        public string Destination { get; set; }
        public decimal DistanceKM { get; set; }
        public string DistanceUnit { get; set; } = "km";
    }
}
