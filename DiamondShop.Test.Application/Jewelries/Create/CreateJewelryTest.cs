using DiamondShop.Application.Dtos.Requests.Jewelries;
using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Usecases.Diamonds.Commands.AttachToJewelry;
using DiamondShop.Application.Usecases.Jewelries.Commands;
using DiamondShop.Application.Usecases.JewelrySideDiamonds.Create;
using DiamondShop.Application.Usecases.MainDiamonds.Commands.CompareDiamondShape;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Infrastructure.Databases;
using DiamondShop.Infrastructure.Databases.Repositories;
using DiamondShop.Infrastructure.Databases.Repositories.JewelryModelRepo;
using DiamondShop.Infrastructure.Databases.Repositories.JewelryRepo;
using DiamondShop.Test.General;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DiamondShop.Test.Application.Jewelries.Create
{
    public class JewelryJSON
    {
        public JewelryRequestDto jewelryRequest { get; set; }
        public List<string> sideDiamondOptIds { get; set; }
        public List<string> attachedDiamondIds { get; set; }
        public bool expectedResult { get; set; }
    }


    [Trait(nameof(Jewelries), "Jewelry")]
    public class CreateJewelryTest
    {
        private readonly Mock<ISender> _sender;

        public CreateJewelryTest()
        {
            _sender = new Mock<ISender>();
        }
        public static IEnumerable<object[]> GetTestData(int skip, int take)
        {
            var jsonData = File.ReadAllText("Data/Jewelry/InputJewelry.json");
            var data = JsonConvert.DeserializeObject<List<JewelryJSON>>(jsonData);
            foreach (var item in data.Skip(skip).Take(take))
            {
                yield return new object[] { item.jewelryRequest, item.sideDiamondOptIds, item.attachedDiamondIds, item.expectedResult };
            }
        }

        [Theory]
        [MemberData(nameof(GetTestData), 1, 1)]
        public async Task Handle_Should_ReturnSuccess_WhenJewelryAddToDb(JewelryRequestDto jewelryRequest, List<string> sideDiamondOptIds, List<string> attachedDiamondIds, bool expectedResult)
        {
            var compareDiamondShapeValidator = new CompareDiamondShapeCommandValidator();
            var attachDiamondValidator = new AttachDiamondCommandValidator();
            var createJewelrySideDiamondValidator = new CreateJewelrySideDiamondCommandValidator();

            DbContextOptions opt = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase($"JewelryTest {new Guid().ToString()}")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            using (var context = new TestDbContext(opt))
            {
                var unitOfWork = new UnitOfWork(context);
                var jewelryCommand = new CreateJewelryCommand(jewelryRequest, sideDiamondOptIds, attachedDiamondIds);

                var handler = new CreateJewelryCommandHandler(new JewelryRepository(context, null), new DiamondRepository(context), _sender.Object, unitOfWork);
                var compareDiamondShapeHandler = new CompareDiamondShapeCommandHandler(new MainDiamondRepository(context));
                var attachDiamondHandler = new AttachDiamondCommandHandler(new DiamondRepository(context), unitOfWork);
                var createJewelrySideDiamondHandler = new CreateJewelrySideDiamondCommandHandler(new JewelrySideDiamondRepository(context), unitOfWork);

                _sender.Setup(s => s.Send(It.IsAny<CompareDiamondShapeCommand>(), It.IsAny<CancellationToken>()))
                    .Returns(async (CompareDiamondShapeCommand command, CancellationToken token) =>
                    {
                        var validate = compareDiamondShapeValidator.Validate(command);
                        if (validate.IsValid)
                            return await compareDiamondShapeHandler.Handle(command, token);
                        else
                            throw new ValidationException(validate.ToString());
                    });

                _sender.Setup(s => s.Send(It.IsAny<AttachDiamondCommand>(), It.IsAny<CancellationToken>()))
                    .Returns(async (AttachDiamondCommand command, CancellationToken token) =>
                     {
                         var validate = attachDiamondValidator.Validate(command);
                         if (validate.IsValid)
                             return await attachDiamondHandler.Handle(command, token);
                         else
                             throw new ValidationException(validate.ToString());
                     });
                _sender.Setup(s => s.Send(It.IsAny<CreateJewelrySideDiamondCommand>(), It.IsAny<CancellationToken>()))
                    .Returns(async (CreateJewelrySideDiamondCommand command, CancellationToken token) =>
                     {
                         var validate = createJewelrySideDiamondValidator.Validate(command);
                         if (validate.IsValid)
                             return await createJewelrySideDiamondHandler.Handle(command, token);
                         else
                             throw new ValidationException(validate.ToString());
                     });
                var result = await handler.Handle(jewelryCommand, default);
                if (expectedResult)
                {
                    result.IsSuccess.Should().BeTrue();
                    var added = context.JewelryModels.AsQueryable();
                    added.Should().HaveCount(1);
                }
                else
                {
                    result.IsSuccess.Should().BeFalse();
                }
            }
        }
    }
}
