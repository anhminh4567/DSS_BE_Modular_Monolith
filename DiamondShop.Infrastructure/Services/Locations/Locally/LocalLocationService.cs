using Azure.Storage.Blobs.Models;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Models;
using DiamondShop.Domain.Common.Addresses;
using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services.Locations.OApi;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Locations.Locally
{
    internal class LocalLocationService : ILocationService
    {
        private readonly ILogger<LocalLocationService> _logger;
        private readonly IOptions<LocationOptions> _locationOptions;
        private readonly OApiLocationService _oApiLocationService;

        public LocalLocationService(ILogger<LocalLocationService> logger, IOptions<LocationOptions> locationOptions, ILogger<OApiLocationService> logger1)
        {
            _logger = logger;
            _locationOptions = locationOptions;
            _oApiLocationService = new OApiLocationService(_locationOptions,logger1) ;
        }

        public Task<Result<LocationDistantData>> GetDistant(string originPlaceId, string destinationPlaceId, CancellationToken cancellationToken = default)
        {
            return _oApiLocationService.GetDistant(originPlaceId, destinationPlaceId, cancellationToken);
        }

        public Task<Result<LocationDistantData>> GetDistant(LocationDetail Origin, LocationDetail Destination, CancellationToken cancellationToken = default)
        {
            return _oApiLocationService.GetDistant(Origin, Destination, cancellationToken);
        }

        public Task<Result<LocationDistantData>> GetDistantFromBaseShopLocation(LocationDetail Destination, CancellationToken cancellationToken = default)
        {
            return _oApiLocationService.GetDistantFromBaseShopLocation(Destination,cancellationToken);
        }

        public List<District> GetDistricts(string provinceId)
        {
            return _oApiLocationService.GetDistricts(provinceId);
        }

        public List<Province> GetProvinces()
        {
            _logger.LogInformation("get all province is called");
            return _allowedProvince.OrderBy(o => o.Id).ToList(); //JsonConvert.DeserializeObject<List<Province>>(_jsonAllowedProvince)!;
        }

        public List<Ward> GetWards(string districtId)
        {
            return _oApiLocationService.GetWards(districtId);
        }

        public decimal ToKm(decimal distanceInMeters)
        {
            return _oApiLocationService.ToKm(distanceInMeters);
        }  
        public static List<Province> ALLOWED_PROVINCE { get => _allowedProvince;  }
        //public static List<Province> JSON_ALLLOWED_PROVINCE = JsonConvert.DeserializeObject<List<Province>>(_jsonAllowedProvince!)!;
        private static string _jsonAllowedProvince = "[{\"Id\":\"1\",\"Name\":\"HàNội\",\"IsActive\":true,\"NameExtension\":[\"HàNội\"]}," +
            "{\"Id\":\"10\",\"Name\":\"LàoCai\",\"IsActive\":true,\"NameExtension\":[\"LàoCai\"]},{\"Id\":\"11\",\"Name\":\"ĐiệnBiên\"," +
            "\"IsActive\":true,\"NameExtension\":[\"ĐiệnBiên\"]},{\"Id\":\"12\",\"Name\":\"LaiChâu\",\"IsActive\":true,\"NameExtension\":[\"LaiChâu\"]}," +
            "{\"Id\":\"14\",\"Name\":\"SơnLa\",\"IsActive\":true,\"NameExtension\":[\"SơnLa\"]},{\"Id\":\"15\",\"Name\":\"YênBái\",\"IsActive\":true,\"NameExtension\":[\"YênBái\"]}," +
            "{\"Id\":\"17\",\"Name\":\"HoàBình\",\"IsActive\":true,\"NameExtension\":[\"HoàBình\"]}," +
            "{\"Id\":\"19\",\"Name\":\"TháiNguyên\",\"IsActive\":true,\"NameExtension\":[\"TháiNguyên\"]}," +
            "{\"Id\":\"2\",\"Name\":\"HàGiang\",\"IsActive\":true,\"NameExtension\":[\"HàGiang\"]}," +
            "{\"Id\":\"20\",\"Name\":\"LạngSơn\",\"IsActive\":true,\"NameExtension\":[\"LạngSơn\"]}," +
            "{\"Id\":\"22\",\"Name\":\"QuảngNinh\",\"IsActive\":true,\"NameExtension\":[\"QuảngNinh\"]}," +
            "{\"Id\":\"24\",\"Name\":\"BắcGiang\",\"IsActive\":true,\"NameExtension\":[\"BắcGiang\"]}," +
            "{\"Id\":\"25\",\"Name\":\"PhúThọ\",\"IsActive\":true,\"NameExtension\":[\"PhúThọ\"]}," +
            "{\"Id\":\"26\",\"Name\":\"VĩnhPhúc\",\"IsActive\":true,\"NameExtension\":[\"VĩnhPhúc\"]}," +
            "{\"Id\":\"27\",\"Name\":\"BắcNinh\",\"IsActive\":true,\"NameExtension\":[\"BắcNinh\"]}," +
            "{\"Id\":\"30\",\"Name\":\"HảiDương\",\"IsActive\":true,\"NameExtension\":[\"HảiDương\"]}," +
            "{\"Id\":\"31\",\"Name\":\"HảiPhòng\",\"IsActive\":true,\"NameExtension\":[\"HảiPhòng\"]}," +
            "{\"Id\":\"33\",\"Name\":\"HưngYên\",\"IsActive\":true,\"NameExtension\":[\"HưngYên\"]}," +
            "{\"Id\":\"34\",\"Name\":\"TháiBình\",\"IsActive\":true,\"NameExtension\":[\"TháiBình\"]}," +
            "{\"Id\":\"35\",\"Name\":\"HàNam\",\"IsActive\":true,\"NameExtension\":[\"HàNam\"]}," +
            "{\"Id\":\"36\",\"Name\":\"NamĐịnh\",\"IsActive\":true,\"NameExtension\":[\"NamĐịnh\"]}," +
            "{\"Id\":\"37\",\"Name\":\"NinhBình\",\"IsActive\":true,\"NameExtension\":[\"NinhBình\"]}," +
            "{\"Id\":\"38\",\"Name\":\"ThanhHóa\",\"IsActive\":true,\"NameExtension\":[\"ThanhHóa\"]}," +
            "{\"Id\":\"4\",\"Name\":\"CaoBằng\",\"IsActive\":true,\"NameExtension\":[\"CaoBằng\"]}," +
            "{\"Id\":\"40\",\"Name\":\"NghệAn\",\"IsActive\":true,\"NameExtension\":[\"NghệAn\"]}," +
            "{\"Id\":\"42\",\"Name\":\"HàTĩnh\",\"IsActive\":true,\"NameExtension\":[\"HàTĩnh\"]}," +
            "{\"Id\":\"44\",\"Name\":\"QuảngBình\",\"IsActive\":true,\"NameExtension\":[\"QuảngBình\"]}," +
            "{\"Id\":\"45\",\"Name\":\"QuảngTrị\",\"IsActive\":true,\"NameExtension\":[\"QuảngTrị\"]}," +
            "{\"Id\":\"46\",\"Name\":\"ThừaThiênHuế\",\"IsActive\":true,\"NameExtension\":[\"ThừaThiênHuế\"]}," +
            "{\"Id\":\"48\",\"Name\":\"ĐàNẵng\",\"IsActive\":true,\"NameExtension\":[\"ĐàNẵng\"]}," +
            "{\"Id\":\"49\",\"Name\":\"QuảngNam\",\"IsActive\":true,\"NameExtension\":[\"QuảngNam\"]}," +
            "{\"Id\":\"51\",\"Name\":\"QuảngNgãi\",\"IsActive\":true,\"NameExtension\":[\"QuảngNgãi\"]}," +
            "{\"Id\":\"52\",\"Name\":\"BìnhĐịnh\",\"IsActive\":true,\"NameExtension\":[\"BìnhĐịnh\"]}," +
            "{\"Id\":\"54\",\"Name\":\"PhúYên\",\"IsActive\":true,\"NameExtension\":[\"PhúYên\"]}," +
            "{\"Id\":\"56\",\"Name\":\"KhánhHòa\",\"IsActive\":true,\"NameExtension\":[\"KhánhHòa\"]}," +
            "{\"Id\":\"58\",\"Name\":\"NinhThuận\",\"IsActive\":true,\"NameExtension\":[\"NinhThuận\"]}," +
            "{\"Id\":\"6\",\"Name\":\"BắcKạn\",\"IsActive\":true,\"NameExtension\":[\"BắcKạn\"]}," +
            "{\"Id\":\"60\",\"Name\":\"BìnhThuận\",\"IsActive\":true,\"NameExtension\":[\"BìnhThuận\"]}," +
            "{\"Id\":\"62\",\"Name\":\"KonTum\",\"IsActive\":true,\"NameExtension\":[\"KonTum\"]}," +
            "{\"Id\":\"64\",\"Name\":\"GiaLai\",\"IsActive\":true,\"NameExtension\":[\"GiaLai\"]}," +
            "{\"Id\":\"66\",\"Name\":\"ĐắkLắk\",\"IsActive\":true,\"NameExtension\":[\"ĐắkLắk\"]}," +
            "{\"Id\":\"67\",\"Name\":\"ĐắkNông\",\"IsActive\":true,\"NameExtension\":[\"ĐắkNông\"]}," +
            "{\"Id\":\"68\",\"Name\":\"LâmĐồng\",\"IsActive\":true,\"NameExtension\":[\"LâmĐồng\"]}," +
            "{\"Id\":\"70\",\"Name\":\"BìnhPhước\",\"IsActive\":true,\"NameExtension\":[\"BìnhPhước\"]}," +
            "{\"Id\":\"72\",\"Name\":\"TâyNinh\",\"IsActive\":true,\"NameExtension\":[\"TâyNinh\"]},{" +
            "\"Id\":\"74\",\"Name\":\"BìnhDương\",\"IsActive\":true,\"NameExtension\":[\"BìnhDương\"]}," +
            "{\"Id\":\"75\",\"Name\":\"ĐồngNai\",\"IsActive\":true,\"NameExtension\":[\"ĐồngNai\"]},{" +
            "\"Id\":\"77\",\"Name\":\"BàRịa-VũngTàu\",\"IsActive\":true,\"NameExtension\":[\"BàRịa-VũngTàu\"]}," +
            "{\"Id\":\"79\",\"Name\":\"HồChíMinh\",\"IsActive\":true,\"NameExtension\":[\"HồChíMinh\"]}," +
            "{\"Id\":\"8\",\"Name\":\"TuyênQuang\",\"IsActive\":true,\"NameExtension\":[\"TuyênQuang\"]}," +
            "{\"Id\":\"80\",\"Name\":\"LongAn\",\"IsActive\":true,\"NameExtension\":[\"LongAn\"]}," +
            "{\"Id\":\"82\",\"Name\":\"TiềnGiang\",\"IsActive\":true,\"NameExtension\":[\"TiềnGiang\"]}," +
            "{\"Id\":\"83\",\"Name\":\"BếnTre\",\"IsActive\":true,\"NameExtension\":[\"BếnTre\"]}," +
            "{\"Id\":\"84\",\"Name\":\"TràVinh\",\"IsActive\":true,\"NameExtension\":[\"TràVinh\"]}," +
            "{\"Id\":\"86\",\"Name\":\"VĩnhLong\",\"IsActive\":true,\"NameExtension\":[\"VĩnhLong\"]}," +
            "{\"Id\":\"87\",\"Name\":\"ĐồngTháp\",\"IsActive\":true,\"NameExtension\":[\"ĐồngTháp\"]}," +
            "{\"Id\":\"89\",\"Name\":\"AnGiang\",\"IsActive\":true,\"NameExtension\":[\"AnGiang\"]}," +
            "{\"Id\":\"91\",\"Name\":\"KiênGiang\",\"IsActive\":true,\"NameExtension\":[\"KiênGiang\"]}," +
            "{\"Id\":\"92\",\"Name\":\"CầnThơ\",\"IsActive\":true,\"NameExtension\":[\"CầnThơ\"]}," +
            "{\"Id\":\"93\",\"Name\":\"HậuGiang\",\"IsActive\":true,\"NameExtension\":[\"HậuGiang\"]}," +
            "{\"Id\":\"94\",\"Name\":\"SócTrăng\",\"IsActive\":true,\"NameExtension\":[\"SócTrăng\"]}," +
            "{\"Id\":\"95\",\"Name\":\"BạcLiêu\",\"IsActive\":true,\"NameExtension\":[\"BạcLiêu\"]}," +
            "{\"Id\":\"96\",\"Name\":\"CàMau\",\"IsActive\":true,\"NameExtension\":[\"CàMau\"]}]";
        private static List<Province> _allowedProvince = new()
        {
            new Province
            {
                Id = "89",
                Name = "An Giang",
                IsActive = true,
                NameExtension = new[] { "An Giang" }
            },
            new Province
            {
                Id = "77",
                Name = "Bà Rịa - Vũng Tàu",
                IsActive = true,
                NameExtension = new[] { "Bà Rịa - Vũng Tàu" }
            },
            new Province
            {
                Id = "24",
                Name = "Bắc Giang",
                IsActive = true,
                NameExtension = new[] { "Bắc Giang" }
            },
            new Province
            {
                Id = "6",
                Name = "Bắc Kạn",
                IsActive = true,
                NameExtension = new[] { "Bắc Kạn" }
            },
            new Province
            {
                Id = "95",
                Name = "Bạc Liêu",
                IsActive = true,
                NameExtension = new[] { "Bạc Liêu" }
            },
            new Province
            {
                Id = "27",
                Name = "Bắc Ninh",
                IsActive = true,
                NameExtension = new[] { "Bắc Ninh" }
            },
            new Province
            {
                Id = "83",
                Name = "Bến Tre",
                IsActive = true,
                NameExtension = new[] { "Bến Tre" }
            },
            new Province
            {
                Id = "52",
                Name = "Bình Định",
                IsActive = true,
                NameExtension = new[] { "Bình Định" }
            },
            new Province
            {
                Id = "74",
                Name = "Bình Dương",
                IsActive = true,
                NameExtension = new[] { "Bình Dương" }
            },
            new Province
            {
                Id = "70",
                Name = "Bình Phước",
                IsActive = true,
                NameExtension = new[] { "Bình Phước" }
            },
            new Province
            {
                Id = "60",
                Name = "Bình Thuận",
                IsActive = true,
                NameExtension = new[] { "Bình Thuận" }
            },
            new Province
            {
                Id = "96",
                Name = "Cà Mau",
                IsActive = true,
                NameExtension = new[] { "Cà Mau" }
            },
            new Province
            {
                Id = "92",
                Name = "Cần Thơ",
                IsActive = true,
                NameExtension = new[] { "Cần Thơ" }
            },
            new Province
            {
                Id = "4",
                Name = "Cao Bằng",
                IsActive = true,
                NameExtension = new[] { "Cao Bằng" }
            },
            new Province
            {
                Id = "48",
                Name = "Đà Nẵng",
                IsActive = true,
                NameExtension = new[] { "Đà Nẵng" }
            },
            new Province
            {
                Id = "66",
                Name = "Đắk Lắk",
                IsActive = true,
                NameExtension = new[] { "Đắk Lắk" }
            },
            new Province
            {
                Id = "67",
                Name = "Đắk Nông",
                IsActive = true,
                NameExtension = new[] { "Đắk Nông" }
            },
            new Province
            {
                Id = "11",
                Name = "Điện Biên",
                IsActive = true,
                NameExtension = new[] { "Điện Biên" }
            },
            new Province
            {
                Id = "75",
                Name = "Đồng Nai",
                IsActive = true,
                NameExtension = new[] { "Đồng Nai" }
            },
            new Province
            {
                Id = "87",
                Name = "Đồng Tháp",
                IsActive = true,
                NameExtension = new[] { "Đồng Tháp" }
            },
            new Province
            {
                Id = "64",
                Name = "Gia Lai",
                IsActive = true,
                NameExtension = new[] { "Gia Lai" }
            },
            new Province
            {
                Id = "2",
                Name = "Hà Giang",
                IsActive = true,
                NameExtension = new[] { "Hà Giang" }
            },
            new Province
            {
                Id = "35",
                Name = "Hà Nam",
                IsActive = true,
                NameExtension = new[] { "Hà Nam" }
            },
            new Province
            {
                Id = "1",
                Name = "Hà Nội",
                IsActive = true,
                NameExtension = new[] { "Hà Nội" }
            },
            new Province
            {
                Id = "42",
                Name = "Hà Tĩnh",
                IsActive = true,
                NameExtension = new[] { "Hà Tĩnh" }
            },
            new Province
            {
                Id = "30",
                Name = "Hải Dương",
                IsActive = true,
                NameExtension = new[] { "Hải Dương" }
            },
            new Province
            {
                Id = "31",
                Name = "Hải Phòng",
                IsActive = true,
                NameExtension = new[] { "Hải Phòng" }
            },
            new Province
            {
                Id = "93",
                Name = "Hậu Giang",
                IsActive = true,
                NameExtension = new[] { "Hậu Giang" }
            },
            new Province
            {
                Id = "79",
                Name = "Hồ Chí Minh",
                IsActive = true,
                NameExtension = new[] { "Hồ Chí Minh" }
            },
            new Province
            {
                Id = "17",
                Name = "Hoà Bình",
                IsActive = true,
                NameExtension = new[] { "Hoà Bình" }
            },
            new Province
            {
                Id = "33",
                Name = "Hưng Yên",
                IsActive = true,
                NameExtension = new[] { "Hưng Yên" }
            },
            new Province
            {
                Id = "56",
                Name = "Khánh Hòa",
                IsActive = true,
                NameExtension = new[] { "Khánh Hòa" }
            },
            new Province
            {
                Id = "91",
                Name = "Kiên Giang",
                IsActive = true,
                NameExtension = new[] { "Kiên Giang" }
            },
            new Province
            {
                Id = "62",
                Name = "Kon Tum",
                IsActive = true,
                NameExtension = new[] { "Kon Tum" }
            },
            new Province
            {
                Id = "12",
                Name = "Lai Châu",
                IsActive = true,
                NameExtension = new[] { "Lai Châu" }
            },
            new Province
            {
                Id = "68",
                Name = "Lâm Đồng",
                IsActive = true,
                NameExtension = new[] { "Lâm Đồng" }
            },
            new Province
            {
                Id = "20",
                Name = "Lạng Sơn",
                IsActive = true,
                NameExtension = new[] { "Lạng Sơn" }
            },
            new Province
            {
                Id = "10",
                Name = "Lào Cai",
                IsActive = true,
                NameExtension = new[] { "Lào Cai" }
            },
            new Province
            {
                Id = "80",
                Name = "Long An",
                IsActive = true,
                NameExtension = new[] { "Long An" }
            },
            new Province
            {
                Id = "36",
                Name = "Nam Định",
                IsActive = true,
                NameExtension = new[] { "Nam Định" }
            },
            new Province
            {
                Id = "40",
                Name = "Nghệ An",
                IsActive = true,
                NameExtension = new[] { "Nghệ An" }
            },
            new Province
            {
                Id = "37",
                Name = "Ninh Bình",
                IsActive = true,
                NameExtension = new[] { "Ninh Bình" }
            },
            new Province
            {
                Id = "58",
                Name = "Ninh Thuận",
                IsActive = true,
                NameExtension = new[] { "Ninh Thuận" }
            },
            new Province
            {
                Id = "25",
                Name = "Phú Thọ",
                IsActive = true,
                NameExtension = new[] { "Phú Thọ" }
            },
            new Province
            {
                Id = "54",
                Name = "Phú Yên",
                IsActive = true,
                NameExtension = new[] { "Phú Yên" }
            },
            new Province
            {
                Id = "44",
                Name = "Quảng Bình",
                IsActive = true,
                NameExtension = new[] { "Quảng Bình" }
            },
            new Province
            {
                Id = "49",
                Name = "Quảng Nam",
                IsActive = true,
                NameExtension = new[] { "Quảng Nam" }
            },
            new Province
            {
                Id = "51",
                Name = "Quảng Ngãi",
                IsActive = true,
                NameExtension = new[] { "Quảng Ngãi" }
            },
            new Province
            {
                Id = "22",
                Name = "Quảng Ninh",
                IsActive = true,
                NameExtension = new[] { "Quảng Ninh" }
            },
            new Province
            {
                Id = "45",
                Name = "Quảng Trị",
                IsActive = true,
                NameExtension = new[] { "Quảng Trị" }
            },
            new Province
            {
                Id = "94",
                Name = "Sóc Trăng",
                IsActive = true,
                NameExtension = new[] { "Sóc Trăng" }
            },
            new Province
            {
                Id = "14",
                Name = "Sơn La",
                IsActive = true,
                NameExtension = new[] { "Sơn La" }
            },
            new Province
            {
                Id = "72",
                Name = "Tây Ninh",
                IsActive = true,
                NameExtension = new[] { "Tây Ninh" }
            },
            new Province
            {
                Id = "34",
                Name = "Thái Bình",
                IsActive = true,
                NameExtension = new[] { "Thái Bình" }
            },
            new Province
            {
                Id = "19",
                Name = "Thái Nguyên",
                IsActive = true,
                NameExtension = new[] { "Thái Nguyên" }
            },
            new Province
            {
                Id = "38",
                Name = "Thanh Hóa",
                IsActive = true,
                NameExtension = new[] { "Thanh Hóa" }
            },
            new Province
            {
                Id = "46",
                Name = "Thừa Thiên Huế",
                IsActive = true,
                NameExtension = new[] { "Thừa Thiên Huế" }
            },
            new Province
            {
                Id = "82",
                Name = "Tiền Giang",
                IsActive = true,
                NameExtension = new[] { "Tiền Giang" }
            },
            new Province
            {
                Id = "84",
                Name = "Trà Vinh",
                IsActive = true,
                NameExtension = new[] { "Trà Vinh" }
            },
            new Province
            {
                Id = "8",
                Name = "Tuyên Quang",
                IsActive = true,
                NameExtension = new[] { "Tuyên Quang" }
            },
            new Province
            {
                Id = "86",
                Name = "Vĩnh Long",
                IsActive = true,
                NameExtension = new[] { "Vĩnh Long" }
            },
            new Province
            {
                Id = "26",
                Name = "Vĩnh Phúc",
                IsActive = true,
                NameExtension = new[] { "Vĩnh Phúc" }
            },
            new Province
            {
                Id = "15",
                Name = "Yên Bái",
                IsActive = true,
                NameExtension = new[] { "Yên Bái" }
            }
        };
    }
}
