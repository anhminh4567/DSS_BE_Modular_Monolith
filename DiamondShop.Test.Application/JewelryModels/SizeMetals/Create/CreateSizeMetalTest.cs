using DiamondShop.Api.Controllers.JewelryModels;
using DiamondShop.Application.Services.Data;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Infrastructure.Databases;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Test.General.JewelryModels.Models.Create
{
    public class CreateSizeMetalTest
    {
        private readonly Mock<ISizeMetalRepository> _sizeMetalRepo;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        public CreateSizeMetalTest()
        {
            _sizeMetalRepo = new Mock<ISizeMetalRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
        }
       
    }
}
