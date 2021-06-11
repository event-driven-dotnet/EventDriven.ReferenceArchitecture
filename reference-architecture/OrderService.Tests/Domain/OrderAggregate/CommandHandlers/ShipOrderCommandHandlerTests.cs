namespace OrderService.Tests.Domain.OrderAggregate.CommandHandlers {

    using System;
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

    public class ShipOrderCommandHandlerTests {

        private readonly Mock<ILogger<ShipOrderCommandHandler>> loggerMoq;
        private readonly Mock<IOrderRepository> repositoryMoq;
        private readonly IMapper mapper;

        public ShipOrderCommandHandlerTests() {
            repositoryMoq = new Mock<IOrderRepository>();
            loggerMoq = new Mock<ILogger<ShipOrderCommandHandler>>();
            mapper = BaseUtils.GetMapper();
        }
        
        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType() {
            var handler = new ShipOrderCommandHandler(repositoryMoq.Object, loggerMoq.Object);

            Assert.NotNull(handler);
            Assert.IsType<ShipOrderCommandHandler>(handler);
        }

        [Fact]
        public async Task WhenOrderDoesNotExist_ThenShouldReturnNotFound() {
            repositoryMoq.Setup(x => x.GetOrder(It.IsAny<Guid>()))
                         .ReturnsAsync((Order) null);
            var handler = new ShipOrderCommandHandler(repositoryMoq.Object, loggerMoq.Object);

            var result = await handler.Handle(new ShipOrder(Guid.Empty, string.Empty), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.NotFound, result.Outcome);
        }

        [Fact]
        public async Task WhenOrderFailsToUpdate_ThenShouldReturnNotFound() {
            var order = mapper.Map<Order>(Orders.Order1);
            repositoryMoq.Setup(x => x.GetOrder(It.IsAny<Guid>()))
                         .ReturnsAsync(order);
            repositoryMoq.Setup(x => x.UpdateOrderState(It.IsAny<Order>(), It.IsAny<OrderState>()))
                         .ReturnsAsync((Order) null);
            var handler = new ShipOrderCommandHandler(repositoryMoq.Object, loggerMoq.Object);

            var result = await handler.Handle(new ShipOrder(Guid.Empty, string.Empty), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.NotFound, result.Outcome);
        }

        [Fact]
        public async Task WhenConcurrencyExceptionOccurs_ThenShouldReturnConflict() {
            var order = mapper.Map<Order>(Orders.Order1);
            repositoryMoq.Setup(x => x.GetOrder(It.IsAny<Guid>()))
                         .ReturnsAsync(order);
            repositoryMoq.Setup(x => x.UpdateOrderState(It.IsAny<Order>(), It.IsAny<OrderState>()))
                         .ThrowsAsync(new ConcurrencyException());
            var handler = new ShipOrderCommandHandler(repositoryMoq.Object, loggerMoq.Object);

            var result = await handler.Handle(new ShipOrder(Guid.Empty, string.Empty), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.Conflict, result.Outcome);
        }

    }

}