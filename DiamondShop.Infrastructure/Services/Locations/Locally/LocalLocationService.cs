using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Models;
using DiamondShop.Domain.Common.Addresses;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Locations.Locally
{
    internal class LocalLocationService : ILocationService
    {
        public Task<Result<LocationDistantData>> GetDistant(string originPlaceId, string destinationPlaceId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<LocationDistantData>> GetDistant(LocationDetail Origin, LocationDetail Destination, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<LocationDistantData>> GetDistantFromBaseShopLocation(LocationDetail Destination, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public List<District> GetDistricts(string provinceId)
        {
            throw new NotImplementedException();
        }

        public List<Province> GetProvinces()
        {
            throw new NotImplementedException();
        }

        public List<Ward> GetWards(string districtId)
        {
            throw new NotImplementedException();
        }

        public decimal ToKm(decimal distanceInMeters)
        {
            throw new NotImplementedException();
        }
    }
}
