using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Test.Domain
{
    public class AnyTest
    {
        [Fact]
        public void TestDateTime()
        {
            var isValidStart = DateTime.TryParseExact(null, "dd-MM-yyyy HH:mm:ss", null, DateTimeStyles.None, out DateTime startDate);
            Assert.False(isValidStart);
        }
    }
}
