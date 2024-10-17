using DiamondShop.Application.Usecases.JewelryModelCategories.Commands.Create;
using DiamondShop.Domain.Models.JewelryModels.Entities;

namespace DiamondShop.Test.Integration
{
    [Trait("Integration", "Category")]
    public class JewelryCategoryTest : BaseIntegrationTest
    {
        public JewelryCategoryTest(IntegrationWAF factory) : base(factory)
        {
        }
        [Fact]
        public async Task Create_NewCategory_Should_AddToDb()
        {
            var command = new CreateJewelryCategoryCommand("Test_Category", "this is a testing category, don't use", true, null);
            var result = await _sender.Send(command);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Create_DuplicateCategory_ShouldNot_AddToDb()
        {
            var seeding = JewelryModelCategory.Create("Test_Category", "this is a testing category, don't use", "", true, null);
            _context.Set<JewelryModelCategory>().Add(seeding);
            await _context.SaveChangesAsync();
            var command = new CreateJewelryCategoryCommand("Test_Category", "this is a testing category, don't use", true, null);
            var result = await _sender.Send(command);
            Assert.True(result.IsFailed);
        }
    }
}
