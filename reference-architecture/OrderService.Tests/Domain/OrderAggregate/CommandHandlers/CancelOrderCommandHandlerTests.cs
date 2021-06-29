using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventDriven.CQRS.Abstractions.Commands;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Domain.OrderAggregate;
using OrderService.Domain.OrderAggregate.CommandHandlers;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Repositories;
using OrderService.Tests.Fakes;
using OrderService.Utils;
using Xunit;

namespace OrderService.Tests.Domain.OrderAggregate.CommandHandlers
{
    public class CancelOrderCommandHandlerTests
    {
        private readonly Mock<ILogger<CancelOrderCommandHandler>> _loggerMoq;
        private readonly IMapper _mapper;

        private readonly Mock<IOrderRepository> _repositoryMoq;

        public CancelOrderCommandHandlerTests()
        {
            _repositoryMoq = new Mock<IOrderRepository>();
            _loggerMoq = new Mock<ILogger<CancelOrderCommandHandler>>();
            _mapper = BaseUtils.GetMapper();
        }

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType()
        {
            var handler = new CancelOrderCommandHandler(_repositoryMoq.Object, _loggerMoq.Object);

            Assert.NotNull(handler);
            Assert.IsType<CancelOrderCommandHandler>(handler);
        }

        [Fact]
        public async Task WhenOrderDoesNotExist_ThenShouldReturnNotFound()
        {
            _repositoryMoq.Setup(x => x.GetOrder(It.IsAny<Guid>()))
                          .ReturnsAsync((Order) null);
            var handler = new CancelOrderCommandHandler(_repositoryMoq.Object, _loggerMoq.Object);

            var result = await handler.Handle(new CancelOrder(Guid.Empty, string.Empty), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.NotFound, result.Outcome);
        }

        [Fact]
        public async Task WhenOrderIsCancelled_ThenResultingOrderStateShouldBeSetToCancelled()
        {
            var order = _mapper.Map<Order>(Orders.Order1);
            _repositoryMoq.Setup(x => x.GetOrder(It.IsAny<Guid>()))
                          .ReturnsAsync(order);
            _repositoryMoq.Setup(x => x.UpdateOrderState(It.IsAny<Order>(), It.IsAny<OrderState>()))
                          .ReturnsAsync(order);
            var handler = new CancelOrderCommandHandler(_repositoryMoq.Object, _loggerMoq.Object);

            var result = await handler.Handle(new CancelOrder(order.Id, order.ETag), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(OrderState.Cancelled, result.Entity.OrderState);
        }

        [Fact]
        public async Task WhenConcurrencyExceptionOccurs_ThenShouldReturnConflict()
        {
            var order = _mapper.Map<Order>(Orders.Order1);
            _repositoryMoq.Setup(x => x.GetOrder(It.IsAny<Guid>()))
                          .ReturnsAsync(order);
            _repositoryMoq.Setup(x => x.UpdateOrderState(It.IsAny<Order>(), It.IsAny<OrderState>()))
                          .ThrowsAsync(new ConcurrencyException());
            var handler = new CancelOrderCommandHandler(_repositoryMoq.Object, _loggerMoq.Object);

            var result = await handler.Handle(new CancelOrder(order.Id, order.ETag), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.Conflict, result.Outcome);
        }
    }
}