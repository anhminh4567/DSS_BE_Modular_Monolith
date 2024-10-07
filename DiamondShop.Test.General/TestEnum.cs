using DiamondShop.Domain.Models.Diamonds.Enums;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace DiamondShop.Test.General
{
    public class TestEnum
    {
        private readonly ITestOutputHelper _output;

        public TestEnum(ITestOutputHelper output)
        {
            _output = output;
        }
        [Fact]
        public void TestEnumValues()
        {
            //write a function to compare the enum values
            string excellent = Polish.Excellent.ToString();
            string veryGOod = Polish.Very_Good.ToString();
            string good = Polish.Good.ToString();
            string fair = Polish.Fair.ToString();
            string poor = Polish.Poor.ToString();
            string[] arrayEnums = { excellent, veryGOod, good, fair, poor };// from largest to smallest 
            var sortedArray =arrayEnums.OrderBy(o => o).ToArray();
            foreach(var item in sortedArray)
            {
                _output.WriteLine(item);
            }
            Assert.True(sortedArray.SequenceEqual(arrayEnums));
        }
        [Fact]
        public void TestEnumValues2()
        {
            string FL = Clarity.FL.ToString();
            string IF = Clarity.IF.ToString();
            string VVS1 = Clarity.VVS1.ToString();
            string VVS2 = Clarity.VVS2.ToString();
            string VS1 = Clarity.VS1.ToString();
            string VS2 = Clarity.VS2.ToString();
            string S11 = Clarity.S11.ToString();
            string S12 = Clarity.S12.ToString();

            string[] arrayEnums = { FL,IF,VVS1,VVS2,VS1,VS2,S11,S12 };// from largest to smallest 
            var sortedArray = arrayEnums.OrderBy(o => o).ToArray();
            foreach (var item in sortedArray)
            {
                _output.WriteLine(item);
            }
            Assert.True(sortedArray.SequenceEqual(arrayEnums));
        }
    }
}
