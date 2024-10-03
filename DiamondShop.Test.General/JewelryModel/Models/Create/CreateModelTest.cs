using DiamondShop.Api.Controllers.JewelryModels;
using DiamondShop.Infrastructure.Databases;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Test.General.JewelryModel.Models.Create
{
    public class CreateModelTest
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        public CreateModelTest(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [Fact]
        public async Task Test_CreateModel()
        {
        /*    using (var context = new DiamondShopDbContext())
            {
                var controller = new JewelryModelController(_sender, _mapper);


                //var result = controller.Create();

                var viewResult = Assert.IsType<ViewResult>(result);

            }*/
        }
    }
}
