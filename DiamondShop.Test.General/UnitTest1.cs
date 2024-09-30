using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Infrastructure.Services.Currencies.OpenExchanges;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using System.Text;

namespace DiamondShop.Test.General
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {

        }
        [Fact]
        public void TestJson()
        {
            
        }
        [Fact]
        public async void TestCurrency()
        {
            CurrencyExchangeService currencyExchangeService = new CurrencyExchangeService();
            Money money = Money.Create("USD",1995.28m);
            var result = await currencyExchangeService.ConvertCurrency(money);
            Assert.True(result.IsSuccess);
        }
        [Fact]
        public void CheckNextChar()
        {
            char a = returnNextChar('A');
            char c = returnNextChar('Y');
            char d = returnNextChar('Z');


            Assert.Equal('B', a);
            Assert.Throws<Exception>(() =>
            {
                returnNextChar('b');
            });
            Assert.Equal('Z', c);
            Assert.Equal('A', d);

        }

        private char returnNextChar(char c)
        {

            if (c < 'A' || c > 'Z')
                throw new Exception("WTF");
            if (c == 'Z')
            {
                return 'A';
            }
            char incremented = (char)(c + 1);
            return incremented;
        }
    }
}