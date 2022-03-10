using System;
using System.Threading;
using System.Threading.Tasks;
using EventDriven.DDD.Abstractions.Commands;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Domain.OrderAggregate.Handlers;
using OrderService.Repositories;
using Xunit;

namespace OrderService.Tests.Domain.OrderAggregate.CommandHandlers
{
    public class RemoveOrderHandlerTests
    {
        private readonly Mock<ILogger<RemoveOrderHandler>> _loggerMoq;
        private readonly Mock<IOrderRepository> _repositoryMoq;

        public RemoveOrderHandlerTests()
        {
            _repositoryMoq = new Mock<IOrderRepository>();
            _loggerMoq = new Mock<ILogger<RemoveOrderHandler>>();
        }

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType()
        {
            var handler = new RemoveOrderHandler(_repositoryMoq.Object, _loggerMoq.Object);

            Assert.NotNull(handler);
            Assert.IsType<RemoveOrderHandler>(handler);
        }

        [Fact]
        public async Task WhenRemoved_ThenShouldReturnAccepted()
        {
            var handler = new RemoveOrderHandler(_repositoryMoq.Object, _loggerMoq.Object);

            var result = await handler.Handle(new RemoveOrder(Guid.Empty), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.Accepted, result.Outcome);
        }
    }
}