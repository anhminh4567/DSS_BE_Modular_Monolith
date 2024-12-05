using DiamondShop.Commons;
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
        private static ConflictError error =  new ConflictError("error1");
        [Fact]
        public void TestDateTime()
        {
            var isValidStart = DateTime.TryParseExact(null, "dd-MM-yyyy HH:mm:ss", null, DateTimeStyles.None, out DateTime startDate);
            Assert.False(isValidStart);
        }
        [Fact]
        public void TestErrorResult()
        {
            var result1 = error;
            var result2 = error;
            Assert.Equal(result1,result2);
            Assert.Equal(result1,error );

        }
    }
}
