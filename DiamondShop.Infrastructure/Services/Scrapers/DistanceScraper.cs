using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Repositories.DeliveryRepo;
using DiamondShop.Infrastructure.Databases;
using DiamondShop.Infrastructure.Databases.Interceptors;
using DiamondShop.Infrastructure.Databases.Repositories.DeliveryRepo;
using HtmlAgilityPack;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDeliveryFeeRepository _deliveryFeeRepository;
        private readonly DiamondShopDbContext _dbContext;

        public DistanceScraper()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DiamondShopDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=DiamondShopTest;Username=postgres;Password=12345;Include Error Detail=true");
            _dbContext = new DiamondShopDbContext(optionsBuilder.Options,new DomainEventsPublishserInterceptors(null,null));
            _unitOfWork = new UnitOfWork(_dbContext);
            _deliveryFeeRepository = new DeliveryFeeRepository(_dbContext);
        }

        public void Execute()
        {
            
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            var driver = new ChromeDriver(options);
            try
            {
                List<DistantFromHCMDto> tobeAddedObject = new();
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
                    var firstDistanceObject = new DistantFromHCMDto() 
                    {
                        Destination = firstCity,
                        DistanceKM = decimal.Parse(firstDistanceKm),
                        DistanceUnit = "km",
                    };
                    string secondCity = null;
                    string secondDistanceKm = null;
                    if (string.IsNullOrEmpty(datas[2].Text.Trim()))
                        secondCity = datas[2].Text.Trim();
                    if (string.IsNullOrEmpty(datas[3].Text.Trim()))
                        secondDistanceKm = datas[3].Text.Trim();
                    if(string.IsNullOrEmpty(secondCity) == false && string.IsNullOrEmpty(secondDistanceKm) == false)
                    {
                        var secondDistanceObject = new DistantFromHCMDto()
                        {
                            Destination = secondCity,
                            DistanceKM = decimal.Parse(secondDistanceKm),
                            DistanceUnit = "km",
                        };
                        tobeAddedObject.Add(secondDistanceObject);
                    }
                    Debug.WriteLine(row.GetAttribute("innerHTML"));
                }
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
