using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Test.Application
{
    public class Initialization
    {
        public CancellationToken CancellationToken { get; set; }
        public Initialization()
        {
            CancellationToken = CancellationToken.None;
        }
    }
    public class BaseData
    {

    }
}
