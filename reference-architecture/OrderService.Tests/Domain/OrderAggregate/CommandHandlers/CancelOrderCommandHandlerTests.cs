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

    public class CancelOrderCommandHandlerTests {

        private readonly Mock<IOrderRepository> repositoryMoq;
        private readonly Mock<ILogger<CancelOrderCommandHandler>> loggerMoq;
        private readonly IMapper mapper;

        public CancelOrderCommandHandlerTests() {
            repositoryMoq = new Mock<IOrderRepository>();
            loggerMoq = new Mock<ILogger<CancelOrderCommandHandler>>();
            mapper = BaseUtils.GetMapper();
        }

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType() {
            var handler = new CancelOrderCommandHandler(repositoryMoq.Object, loggerMoq.Object);

            Assert.NotNull(handler);
            Assert.IsType<CancelOrderCommandHandler>(handler);
        }

        [Fact]
        public async Task WhenOrderDoesNotExist_ThenShouldReturnNotFound() {
            repositoryMoq.Setup(x => x.GetOrder(It.IsAny<Guid>()))
                         .ReturnsAsync((Order) null);
            var handler = new CancelOrderCommandHandler(repositoryMoq.Object, loggerMoq.Object);

            var result = await handler.Handle(new CancelOrder(Guid.Empty, string.Empty), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.NotFound, result.Outcome);
        }

        [Fact]
        public async Task WhenOrderIsCancelled_ThenResultingOrderStateShouldBeSetToCancelled() {
            var order = mapper.Map<Order>(Orders.Order1);
            repositoryMoq.Setup(x => x.GetOrder(It.IsAny<Guid>()))
                         .ReturnsAsync(order);
            repositoryMoq.Setup(x => x.UpdateOrderState(It.IsAny<Order>(), It.IsAny<OrderState>()))
                         .ReturnsAsync(order);
            var handler = new CancelOrderCommandHandler(repositoryMoq.Object, loggerMoq.Object);

            var result = await handler.Handle(new CancelOrder(order.Id, order.ETag), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(OrderState.Cancelled, result.Entity.OrderState);
        }

        [Fact]
        public async Task WhenConcurrencyExceptionOccurs_ThenShouldReturnConflict() {
            var order = mapper.Map<Order>(Orders.Order1);
            repositoryMoq.Setup(x => x.GetOrder(It.IsAny<Guid>()))
                         .ReturnsAsync(order);
            repositoryMoq.Setup(x => x.UpdateOrderState(It.IsAny<Order>(), It.IsAny<OrderState>()))
                         .ThrowsAsync(new ConcurrencyException());
            var handler = new CancelOrderCommandHandler(repositoryMoq.Object, loggerMoq.Object);

            var result = await handler.Handle(new CancelOrder(order.Id, order.ETag), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.Conflict, result.Outcome);
        }

    }

}