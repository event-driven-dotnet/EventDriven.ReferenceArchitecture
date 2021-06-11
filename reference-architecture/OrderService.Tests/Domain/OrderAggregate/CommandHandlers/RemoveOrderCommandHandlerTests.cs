namespace OrderService.Tests.Domain.OrderAggregate.CommandHandlers {

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

    public class RemoveOrderCommandHandlerTests {

        private readonly Mock<ILogger<RemoveOrderCommandHandler>> loggerMoq;
        private readonly Mock<IOrderRepository> repositoryMoq;

        public RemoveOrderCommandHandlerTests() {
            repositoryMoq = new Mock<IOrderRepository>();
            loggerMoq = new Mock<ILogger<RemoveOrderCommandHandler>>();
        }
        
        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType() {
            var handler = new RemoveOrderCommandHandler(repositoryMoq.Object, loggerMoq.Object);

            Assert.NotNull(handler);
            Assert.IsType<RemoveOrderCommandHandler>(handler);
        }

        [Fact]
        public async Task WhenRemoved_ThenShouldReturnAccepted() {
            var handler = new RemoveOrderCommandHandler(repositoryMoq.Object, loggerMoq.Object);

            var result = await handler.Handle(new RemoveOrder(Guid.Empty), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.Accepted, result.Outcome);
        }

    }

}