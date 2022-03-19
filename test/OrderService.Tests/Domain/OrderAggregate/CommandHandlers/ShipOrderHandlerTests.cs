using System;
using System.Threading.Tasks;
using AutoMapper;
using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Repositories;
using Moq;
using OrderService.Domain.OrderAggregate;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Domain.OrderAggregate.CommandHandlers;
using OrderService.Repositories;
using OrderService.Tests.Fakes;
using OrderService.Tests.Helpers;
using Xunit;

namespace OrderService.Tests.Domain.OrderAggregate.CommandHandlers
{
    public class ShipOrderHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IOrderRepository> _repositoryMoq;

        public ShipOrderHandlerTests()
        {
            _repositoryMoq = new Mock<IOrderRepository>();
            _mapper = MappingHelper.GetMapper();
        }

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType()
        {
            var handler = new ShipOrderHandler(_repositoryMoq.Object);

            Assert.NotNull(handler);
            Assert.IsType<ShipOrderHandler>(handler);
        }

        [Fact]
        public async Task WhenOrderDoesNotExist_ThenShouldReturnNotFound()
        {
            _repositoryMoq.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Order) null!);
            var handler = new ShipOrderHandler(_repositoryMoq.Object);

            var result = await handler.Handle(new ShipOrder(Guid.Empty), default);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.NotFound, result.Outcome);
        }

        [Fact]
        public async Task WhenOrderFailsToUpdate_ThenShouldReturnNotFound()
        {
            var order = _mapper.Map<Order>(Orders.Order1);
            _repositoryMoq.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(order);
            _repositoryMoq.Setup(x => x.UpdateOrderStateAsync(It.IsAny<Order>(), It.IsAny<OrderState>()))
                .ReturnsAsync((Order) null!);
            var handler = new ShipOrderHandler(_repositoryMoq.Object);

            var result = await handler.Handle(new ShipOrder(Guid.Empty), default);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.NotFound, result.Outcome);
        }

        [Fact]
        public async Task WhenConcurrencyExceptionOccurs_ThenShouldReturnConflict()
        {
            var order = _mapper.Map<Order>(Orders.Order1);
            _repositoryMoq.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(order);
            _repositoryMoq.Setup(x => x.UpdateOrderStateAsync(It.IsAny<Order>(), It.IsAny<OrderState>()))
                .ThrowsAsync(new ConcurrencyException());
            var handler = new ShipOrderHandler(_repositoryMoq.Object);

            var result = await handler.Handle(new ShipOrder(Guid.Empty), default);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.Conflict, result.Outcome);
        }
    }
}