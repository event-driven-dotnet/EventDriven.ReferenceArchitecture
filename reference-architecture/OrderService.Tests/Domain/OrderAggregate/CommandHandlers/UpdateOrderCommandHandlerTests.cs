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

    public class UpdateOrderCommandHandlerTests {

        private readonly Mock<IOrderRepository> repositoryMoq;
        private readonly Mock<ILogger<UpdateOrderCommandHandler>> loggerMoq;
        private readonly IMapper mapper;

        public UpdateOrderCommandHandlerTests() {
            repositoryMoq = new Mock<IOrderRepository>();
            loggerMoq = new Mock<ILogger<UpdateOrderCommandHandler>>();
            mapper = BaseUtils.GetMapper();
        }
        
        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType() {
            var handler = new UpdateOrderCommandHandler(repositoryMoq.Object, loggerMoq.Object);

            Assert.NotNull(handler);
            Assert.IsType<UpdateOrderCommandHandler>(handler);
        }

        [Fact]
        public async Task WhenOrderFailsToUpdate_ThenShouldReturnNotFound() {
            repositoryMoq.Setup(x => x.UpdateOrder(It.IsAny<Order>()))
                         .ReturnsAsync((Order) null);
            var handler = new UpdateOrderCommandHandler(repositoryMoq.Object, loggerMoq.Object);

            var result = await handler.Handle(new UpdateOrder(new Order()), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.NotFound, result.Outcome);
        }

        [Fact]
        public async Task WhenConcurrencyExceptionOccurs_ThenShouldReturnConflict() {
            repositoryMoq.Setup(x => x.UpdateOrder(It.IsAny<Order>()))
                         .ThrowsAsync(new ConcurrencyException());
            var handler = new UpdateOrderCommandHandler(repositoryMoq.Object, loggerMoq.Object);

            var result = await handler.Handle(new UpdateOrder(new Order()), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.Conflict, result.Outcome);
        }

        [Fact]
        public async Task WhenOrderIsUpdated_ThenShouldReturnAcceptedEntity() {
            var order = mapper.Map<Order>(Orders.Order1);
            repositoryMoq.Setup(x => x.UpdateOrder(It.IsAny<Order>()))
                         .ReturnsAsync(order);
            var handler = new UpdateOrderCommandHandler(repositoryMoq.Object, loggerMoq.Object);

            var result = await handler.Handle(new UpdateOrder(order), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.Accepted, result.Outcome);
            Assert.Equal(order.Id, result.Entity.Id);
        }

    }

}