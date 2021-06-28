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
    public class UpdateOrderCommandHandlerTests
    {
        private readonly Mock<ILogger<UpdateOrderCommandHandler>> _loggerMoq;
        private readonly IMapper _mapper;

        private readonly Mock<IOrderRepository> _repositoryMoq;

        public UpdateOrderCommandHandlerTests()
        {
            _repositoryMoq = new Mock<IOrderRepository>();
            _loggerMoq = new Mock<ILogger<UpdateOrderCommandHandler>>();
            _mapper = BaseUtils.GetMapper();
        }

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType()
        {
            var handler = new UpdateOrderCommandHandler(_repositoryMoq.Object, _loggerMoq.Object);

            Assert.NotNull(handler);
            Assert.IsType<UpdateOrderCommandHandler>(handler);
        }

        [Fact]
        public async Task WhenOrderFailsToUpdate_ThenShouldReturnNotFound()
        {
            _repositoryMoq.Setup(x => x.UpdateOrder(It.IsAny<Order>()))
                          .ReturnsAsync((Order) null);
            var handler = new UpdateOrderCommandHandler(_repositoryMoq.Object, _loggerMoq.Object);

            var result = await handler.Handle(new UpdateOrder(new Order()), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.NotFound, result.Outcome);
        }

        [Fact]
        public async Task WhenConcurrencyExceptionOccurs_ThenShouldReturnConflict()
        {
            _repositoryMoq.Setup(x => x.UpdateOrder(It.IsAny<Order>()))
                          .ThrowsAsync(new ConcurrencyException());
            var handler = new UpdateOrderCommandHandler(_repositoryMoq.Object, _loggerMoq.Object);

            var result = await handler.Handle(new UpdateOrder(new Order()), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.Conflict, result.Outcome);
        }

        [Fact]
        public async Task WhenOrderIsUpdated_ThenShouldReturnAcceptedEntity()
        {
            var order = _mapper.Map<Order>(Orders.Order1);
            _repositoryMoq.Setup(x => x.UpdateOrder(It.IsAny<Order>()))
                          .ReturnsAsync(order);
            var handler = new UpdateOrderCommandHandler(_repositoryMoq.Object, _loggerMoq.Object);

            var result = await handler.Handle(new UpdateOrder(order), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.Accepted, result.Outcome);
            Assert.Equal(order.Id, result.Entity.Id);
        }
    }
}