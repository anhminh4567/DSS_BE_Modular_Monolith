using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Models;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.Addresses;
using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services.Locations.Models;
using DiamondShop.Infrastructure.Services.Locations.OpenApiProvinces.Models;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Locations
{
    internal class OpenApiProvinceLocationService : ILocationService
    {
        private const string baseUrl = "https://provinces.open-api.vn/api";
        //this is a key, no worry, free tier, no billing attach;
        private const string SERPER = "52662a35ab28700fa0fe01ef511f4db35db5a3f2";
        private const string SERPER_MAP_URL = "https://google.serper.dev/maps";
        //this is a key, no worry, free tier, no billing attach;
        private const string GOOGLE = "AIzaSyCbT6hCrpFqLQKVFbBTSPSkFPpuYTqcWWU";
        private const string GOOGLE_MAP_URL = "https://maps.googleapis.com/maps/api/distancematrix/json"; 
       private readonly ILogger<OpenApiProvinceLocationService> _logger;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public OpenApiProvinceLocationService(ILogger<OpenApiProvinceLocationService> logger, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _logger = logger;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<LocationDistantData>> GetDistant(LocationDetail origin, LocationDetail destination, CancellationToken cancellationToken = default)
        {
            if (origin == null || destination == null || origin.Ward == null || origin.Road == null || origin.District == null || origin.Province == null || destination.Ward == null || destination.Road == null || destination.District == null || destination.Province == null)
            {
                return Result.Fail("null arguments, make sure all is not null");
            }
            var destinationPlaceId = GetPlaceId(destination).Result;
            var originPlaceId = GetPlaceId(origin).Result;
            var calculateDistanceResult = await GetDistanceFromPlaceId(originPlaceId, destinationPlaceId);
            if(calculateDistanceResult.IsSuccess)
                return calculateDistanceResult;
            else
                return Result.Fail(calculateDistanceResult.Errors);
        }
        private async Task<string> GetPlaceId(LocationDetail locationDetail)
        {
            var httpClient = new HttpClient();
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{SERPER_MAP_URL}");
            httpRequest.Headers.Add("X-API-KEY",SERPER);
            var jsonStringBody = JsonConvert.SerializeObject(new 
            {
                q = $"{locationDetail.Road}, {locationDetail.Ward}, {locationDetail.District}, {locationDetail.Province}",
            });

            httpRequest.Content = new StringContent(jsonStringBody, Encoding.UTF8, "application/json");
            var result = await httpClient.SendAsync(httpRequest);
            if (result.IsSuccessStatusCode)
            {
                var mappedResult = await result.Content.ReadFromJsonAsync<SerperRootResponse>();
                httpClient.Dispose();
                return mappedResult.Places.First().PlaceId;
            }
            else
                throw new Exception("api end with error status code: " + result.StatusCode.ToString());
        }
        private async Task<Result<LocationDistantData>> GetDistanceFromPlaceId(string source, string destination)
        {
            _logger.LogInformation("the method GetDistanceFromPlaceId() i called with original place id = {0} and destination place id = {1}", source, destination);
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(destination);
            string url = $"{GOOGLE_MAP_URL}?origins=place_id:{source}&destinations=place_id:{destination}&key={GOOGLE}";
            var httpClient = new HttpClient();
            var result = await httpClient.GetAsync(url);
            if (result.IsSuccessStatusCode)
            {
                var mappedResult = await result.Content.ReadFromJsonAsync<GoogleDistanceMatrixResponse>();
                if (mappedResult.Status != "OK")
                    return Result.Fail("api end with error status message: " + mappedResult.Status);
                httpClient.Dispose();
                var destinationAddress = mappedResult.DestinationAddresses.First();
                var originAddress = mappedResult.OriginAddresses.First();
                var element = mappedResult.Rows.First().Elements.First();
                var distance = element.Distance;
                var duration = element.Duration;
                var elementStatus = element.Status;
                //if(element.Status != "OK")
                //    throw new Exception("api end with error status message from element status, check api to see error: " + elementStatus);
                var distanceKilometer = decimal.Divide((decimal)distance.Value, 1000m);
                var durationHour = decimal.Divide((decimal)duration.Value, 3600m);
                return new LocationDistantData()
                {
                    Distance = distanceKilometer,
                    DistanceUnit = "Km",
                    TravelTime = durationHour,
                    TravelTimeUnit = "Giờ",
                    TravelMode = "cơ giới",
                    Origin = ParseFromAddress(originAddress),
                    Destination = ParseFromAddress(destinationAddress)
                };
            }
            else
                return Result.Fail("api end with error status code: " + result.StatusCode.ToString());
            
             LocationDetail ParseFromAddress(string address)
            {
                var addressParts = address.Split(',',StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
                if (addressParts.Last().ToUpper().Contains("Vietnam".ToUpper()))
                {
                    addressParts.RemoveAt(addressParts.Count - 1);
                }
                return new LocationDetail()
                {
                    Province = addressParts[addressParts.Count - 1],
                    District = addressParts[addressParts.Count - 2],
                    Ward = addressParts[addressParts.Count - 3],
                    Road = address
                };
            }

        }
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

        public async Task<Result<LocationDistantData>> GetDistantFromBaseShopLocation(LocationDetail Destination, CancellationToken cancellationToken = default)
        {
            var shopBaseAddress = _optionsMonitor.CurrentValue.LocationRules; //_locationOptions.Value.ShopOrignalLocation;
            var shopPlaceId = shopBaseAddress.OrinalPlaceId;
            var destinationId = await GetPlaceId(Destination);
            if(shopPlaceId == null || destinationId == null)
            {
                return Result.Fail("null result for des and shop id, make sure all is not null");
            }
            return await GetDistant(shopPlaceId, destinationId, cancellationToken);
        }

        public async Task<Result<LocationDistantData>> GetDistant(string originPlaceId, string destinationPlaceId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("the method is called with original place id = {0} and destination place id = {1}", originPlaceId, destinationPlaceId);
            var calculateDistanceResult = await GetDistanceFromPlaceId(originPlaceId, destinationPlaceId);
            if (calculateDistanceResult.IsSuccess)
                return calculateDistanceResult;
            else
                return Result.Fail(calculateDistanceResult.Errors);
        }

        public decimal ToKm(decimal distanceInMeters)
        {
            ArgumentNullException.ThrowIfNull(distanceInMeters);
            return Math.Round(decimal.Divide(distanceInMeters, 1000m), 2);
        }

        public LocationDetail GetShopLocation(string? shopName)
        {
            throw new NotImplementedException();
        }
    }
}
