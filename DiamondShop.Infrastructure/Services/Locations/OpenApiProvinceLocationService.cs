using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common.Addresses;
using DiamondShop.Infrastructure.Services.Locations.Models;
using OpenQA.Selenium.Internal;
using Syncfusion.XlsIO.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Locations
{
    internal class OpenApiProvinceLocationService : ILocationService
    {
        private readonly string baseUrl = "https://provinces.open-api.vn/api";
        public List<District> GetDistricts(string provinceId)
        {
            ArgumentNullException.ThrowIfNull(provinceId);
            var httpClient = new HttpClient();
            var httpRequest = new HttpRequestMessage();

            httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/d/search/?q=*&p={int.Parse(provinceId)}");
            httpRequest.Content = new StringContent("", Encoding.UTF8, "application/json");

            var result = httpClient.SendAsync(httpRequest).Result;
            if (result.IsSuccessStatusCode)
            {
                var mappedResult = result.Content.ReadFromJsonAsync<List<OpenApiDistrict>>().Result.Select(d => new District()
                {
                    Id = d.Code.ToString(),
                    Name = d.Name,
                    IsActive = true,
                    NameExtension = new string[] { d.Name }
                }).ToList();
                httpClient.Dispose();
                return mappedResult;
            }
            else
                throw new Exception("api end with error status code: " + result.StatusCode.ToString());
        }

        public List<Province> GetProvinces()
        {
            var httpClient = new HttpClient();
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/p");
            httpRequest.Content = new StringContent("", Encoding.UTF8, "application/json");
            var result = httpClient.SendAsync(httpRequest).Result;
            var mappedResult = result.Content.ReadFromJsonAsync<List<OpenApiProvince>>().Result.Select(p => new Province()
            {
                Id = p.Code.ToString(),
                Name = p.Name,
                IsActive = true,
                NameExtension = new string[] { p.Name, p.Codename }
            }).ToList();
            httpClient.Dispose();
            return mappedResult;
        }

        public List<Ward> GetWards(string districtId)
        {
            ArgumentNullException.ThrowIfNull(districtId);
            var httpClient = new HttpClient();
            var httpRequest = new HttpRequestMessage();
            httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/w/search/?q=*&d={int.Parse(districtId)}");
            httpRequest.Content = new StringContent("", Encoding.UTF8, "application/json");
            var result = httpClient.SendAsync(httpRequest).Result;
            if (result.IsSuccessStatusCode)
            {
                var mappedResult = result.Content.ReadFromJsonAsync<List<OpenApiWard>>().Result.Select(d => new Ward()
                {
                    Id = d.Code.ToString(),
                    Name = d.Name,
                    IsActive = true,
                    NameExtension = new string[] { d.Name }
                }).ToList();
                httpClient.Dispose();
                return mappedResult;
            }
            else
                throw new Exception("api end with error status code: " + result.StatusCode.ToString());
        }

    }
}
