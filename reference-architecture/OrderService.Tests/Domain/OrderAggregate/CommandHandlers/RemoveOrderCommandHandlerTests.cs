using System;
using System.Threading;
using System.Threading.Tasks;
using EventDriven.CQRS.Abstractions.Commands;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Domain.OrderAggregate.CommandHandlers;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Repositories;
using Xunit;

namespace OrderService.Tests.Domain.OrderAggregate.CommandHandlers
{
    public class RemoveOrderCommandHandlerTests
    {
        private readonly Mock<ILogger<RemoveOrderCommandHandler>> _loggerMoq;
        private readonly Mock<IOrderRepository> _repositoryMoq;

        public RemoveOrderCommandHandlerTests()
        {
            _repositoryMoq = new Mock<IOrderRepository>();
            _loggerMoq = new Mock<ILogger<RemoveOrderCommandHandler>>();
        }

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType()
        {
            var handler = new RemoveOrderCommandHandler(_repositoryMoq.Object, _loggerMoq.Object);

            Assert.NotNull(handler);
            Assert.IsType<RemoveOrderCommandHandler>(handler);
        }

        [Fact]
        public async Task WhenRemoved_ThenShouldReturnAccepted()
        {
            var handler = new RemoveOrderCommandHandler(_repositoryMoq.Object, _loggerMoq.Object);

            var result = await handler.Handle(new RemoveOrder(Guid.Empty), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.Accepted, result.Outcome);
        }
    }
}