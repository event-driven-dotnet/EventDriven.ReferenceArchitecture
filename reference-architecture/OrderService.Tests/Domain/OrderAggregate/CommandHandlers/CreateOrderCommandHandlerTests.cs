namespace OrderService.Tests.Domain.OrderAggregate.CommandHandlers {

    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using EventDriven.CQRS.Abstractions.Commands;
    using Fakes;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrderService.Domain.OrderAggregate;
    using OrderService.Domain.OrderAggregate.CommandHandlers;
    using OrderService.Domain.OrderAggregate.Commands;
    using OrderService.Repositories;
    using Utils;
    using Xunit;

    public class CreateOrderCommandHandlerTests {

        private readonly Mock<IOrderRepository> repositoryMoq;
        private readonly Mock<ILogger<CreateOrderCommandHandler>> loggerMoq;
        private readonly IMapper mapper;

        public CreateOrderCommandHandlerTests() {
            repositoryMoq = new Mock<IOrderRepository>();
            loggerMoq = new Mock<ILogger<CreateOrderCommandHandler>>();
            mapper = BaseUtils.GetMapper();
        }
        
        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType() {
            var handler = new CreateOrderCommandHandler(repositoryMoq.Object, loggerMoq.Object);

            Assert.NotNull(handler);
            Assert.IsType<CreateOrderCommandHandler>(handler);
        }

        [Fact]
        public async Task WhenOrderDoesNotSaveCorrectly_ThenShouldReturnInvalidCommand() {
            repositoryMoq.Setup(x => x.AddOrder(It.IsAny<Order>()))
                         .ReturnsAsync((Order) null);
            var handler = new CreateOrderCommandHandler(repositoryMoq.Object, loggerMoq.Object);

            var result = await handler.Handle(new CreateOrder(mapper.Map<Order>(Orders.Order1)), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.InvalidCommand, result.Outcome);
        }

        [Fact]
        public async Task WhenOrderIsCreated_ThenShouldReturnEntity() {
            var order = mapper.Map<Order>(Orders.Order1);
            repositoryMoq.Setup(x => x.AddOrder(It.IsAny<Order>()))
                         .ReturnsAsync(order);
            var handler = new CreateOrderCommandHandler(repositoryMoq.Object, loggerMoq.Object);

            var result = await handler.Handle(new CreateOrder(order), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.Accepted, result.Outcome);
            Assert.Equal(order.Id, result.Entity.Id);
        }

    }

}