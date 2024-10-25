using DiamondShop.Domain.Models.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.Cities
{
    internal class AppProvinceConfiguration : IEntityTypeConfiguration<AppProvince>
    {
        public void Configure(EntityTypeBuilder<AppProvince> builder)
        {
            builder.HasData(_allowedProvinces);
            builder.HasIndex(o => o.Name);
            builder.HasKey(o => o.Id);
        }
        private static List<AppProvince> _allowedProvinces = new()
        {
            new AppProvince
            {
                Id = "89",
                Name = "An Giang",
                ApiId = "89",  // Assuming ApiId would be the same as Id in this case
                IsActive = true
            },
            new AppProvince
            {
                Id = "77",
                Name = "Bà Rịa - Vũng Tàu",
                ApiId = "77",
                IsActive = true
            },
            new AppProvince
            {
                Id = "24",
                Name = "Bắc Giang",
                ApiId = "24",
                IsActive = true
            },
            new AppProvince
            {
                Id = "6",
                Name = "Bắc Kạn",
                ApiId = "6",
                IsActive = true
            },
            new AppProvince
            {
                Id = "95",
                Name = "Bạc Liêu",
                ApiId = "95",
                IsActive = true
            },
            new AppProvince
            {
                Id = "27",
                Name = "Bắc Ninh",
                ApiId = "27",
                IsActive = true
            },
            new AppProvince
            {
                Id = "83",
                Name = "Bến Tre",
                ApiId = "83",
                IsActive = true
            },
            new AppProvince
            {
                Id = "52",
                Name = "Bình Định",
                ApiId = "52",
                IsActive = true
            },
            new AppProvince
            {
                Id = "74",
                Name = "Bình Dương",
                ApiId = "74",
                IsActive = true
            },
            new AppProvince
            {
                Id = "70",
                Name = "Bình Phước",
                ApiId = "70",
                IsActive = true
            },
            new AppProvince
            {
                Id = "60",
                Name = "Bình Thuận",
                ApiId = "60",
                IsActive = true
            },
            new AppProvince
            {
                Id = "96",
                Name = "Cà Mau",
                ApiId = "96",
                IsActive = true
            },
            new AppProvince
            {
                Id = "92",
                Name = "Cần Thơ",
                ApiId = "92",
                IsActive = true
            },
            new AppProvince
            {
                Id = "4",
                Name = "Cao Bằng",
                ApiId = "4",
                IsActive = true
            },
            new AppProvince
            {
                Id = "48",
                Name = "Đà Nẵng",
                ApiId = "48",
                IsActive = true
            },
            new AppProvince
            {
                Id = "66",
                Name = "Đắk Lắk",
                ApiId = "66",
                IsActive = true
            },
            new AppProvince
            {
                Id = "67",
                Name = "Đắk Nông",
                ApiId = "67",
                IsActive = true
            },
            new AppProvince
            {
                Id = "11",
                Name = "Điện Biên",
                ApiId = "11",
                IsActive = true
            },
            new AppProvince
            {
                Id = "75",
                Name = "Đồng Nai",
                ApiId = "75",
                IsActive = true
            },
            new AppProvince
            {
                Id = "87",
                Name = "Đồng Tháp",
                ApiId = "87",
                IsActive = true
            },
            new AppProvince
            {
                Id = "64",
                Name = "Gia Lai",
                ApiId = "64",
                IsActive = true
            },
            new AppProvince
            {
                Id = "2",
                Name = "Hà Giang",
                ApiId = "2",
                IsActive = true
            },
            new AppProvince
            {
                Id = "35",
                Name = "Hà Nam",
                ApiId = "35",
                IsActive = true
            },
            new AppProvince
            {
                Id = "1",
                Name = "Hà Nội",
                ApiId = "1",
                IsActive = true
            },
            new AppProvince
            {
                Id = "42",
                Name = "Hà Tĩnh",
                ApiId = "42",
                IsActive = true
            },
            new AppProvince
            {
                Id = "30",
                Name = "Hải Dương",
                ApiId = "30",
                IsActive = true
            },
            new AppProvince
            {
                Id = "31",
                Name = "Hải Phòng",
                ApiId = "31",
                IsActive = true
            },
            new AppProvince
            {
                Id = "93",
                Name = "Hậu Giang",
                ApiId = "93",
                IsActive = true
            },
            new AppProvince
            {
                Id = "79",
                Name = "Hồ Chí Minh",
                ApiId = "79",
                IsActive = true
            },
            new AppProvince
            {
                Id = "17",
                Name = "Hoà Bình",
                ApiId = "17",
                IsActive = true
            },
            new AppProvince
            {
                Id = "33",
                Name = "Hưng Yên",
                ApiId = "33",
                IsActive = true
            },
            new AppProvince
            {
                Id = "56",
                Name = "Khánh Hòa",
                ApiId = "56",
                IsActive = true
            },
            new AppProvince
            {
                Id = "91",
                Name = "Kiên Giang",
                ApiId = "91",
                IsActive = true
            },
            new AppProvince
            {
                Id = "62",
                Name = "Kon Tum",
                ApiId = "62",
                IsActive = true
            },
            new AppProvince
            {
                Id = "12",
                Name = "Lai Châu",
                ApiId = "12",
                IsActive = true
            },
            new AppProvince
            {
                Id = "68",
                Name = "Lâm Đồng",
                ApiId = "68",
                IsActive = true
            },
            new AppProvince
            {
                Id = "20",
                Name = "Lạng Sơn",
                ApiId = "20",
                IsActive = true
            },
            new AppProvince
            {
                Id = "10",
                Name = "Lào Cai",
                ApiId = "10",
                IsActive = true
            },
            new AppProvince
            {
                Id = "80",
                Name = "Long An",
                ApiId = "80",
                IsActive = true
            },
            new AppProvince
            {
                Id = "36",
                Name = "Nam Định",
                ApiId = "36",
                IsActive = true
            },
            new AppProvince
            {
                Id = "40",
                Name = "Nghệ An",
                ApiId = "40",
                IsActive = true
            },
            new AppProvince
            {
                Id = "37",
                Name = "Ninh Bình",
                ApiId = "37",
                IsActive = true
            },
            new AppProvince
            {
                Id = "58",
                Name = "Ninh Thuận",
                ApiId = "58",
                IsActive = true
            },
            new AppProvince
            {
                Id = "25",
                Name = "Phú Thọ",
                ApiId = "25",
                IsActive = true
            },
            new AppProvince
            {
                Id = "54",
                Name = "Phú Yên",
                ApiId = "54",
                IsActive = true
            },
            new AppProvince
            {
                Id = "44",
                Name = "Quảng Bình",
                ApiId = "44",
                IsActive = true
            },
            new AppProvince
            {
                Id = "49",
                Name = "Quảng Nam",
                ApiId = "49",
                IsActive = true
            },
            new AppProvince
            {
                Id = "51",
                Name = "Quảng Ngãi",
                ApiId = "51",
                IsActive = true
            },
            new AppProvince
            {
                Id = "22",
                Name = "Quảng Ninh",
                ApiId = "22",
                IsActive = true
            },
            new AppProvince
            {
                Id = "45",
                Name = "Quảng Trị",
                ApiId = "45",
                IsActive = true
            },
            new AppProvince
            {
                Id = "94",
                Name = "Sóc Trăng",
                ApiId = "94",
                IsActive = true
            },
            new AppProvince
            {
                Id = "14",
                Name = "Sơn La",
                ApiId = "14",
                IsActive = true
            },
            new AppProvince
            {
                Id = "72",
                Name = "Tây Ninh",
                ApiId = "72",
                IsActive = true
            },
            new AppProvince
            {
                Id = "34",
                Name = "Thái Bình",
                ApiId = "34",
                IsActive = true
            },
            new AppProvince
            {
                Id = "19",
                Name = "Thái Nguyên",
                ApiId = "19",
                IsActive = true
            },
            new AppProvince
            {
                Id = "38",
                Name = "Thanh Hóa",
                ApiId = "38",
                IsActive = true
            },
            new AppProvince
            {
                Id = "46",
                Name = "Thừa Thiên Huế",
                ApiId = "46",
                IsActive = true
            },
            new AppProvince
            {
                Id = "82",
                Name = "Tiền Giang",
                ApiId = "82",
                IsActive = true
            },
            new AppProvince
            {
                Id = "84",
                Name = "Trà Vinh",
                ApiId = "84",
                IsActive = true
            },
            new AppProvince
            {
                Id = "8",
                Name = "Tuyên Quang",
                ApiId = "8",
                IsActive = true
            },
            new AppProvince
            {
                Id = "86",
                Name = "Vĩnh Long",
                ApiId = "86",
                IsActive = true
            },
            new AppProvince
            {
                Id = "26",
                Name = "Vĩnh Phúc",
                ApiId = "26",
                IsActive = true
            },
            new AppProvince
            {
                Id = "15",
                Name = "Yên Bái",
                ApiId = "15",
                IsActive = true
            }
        };
    }
}
