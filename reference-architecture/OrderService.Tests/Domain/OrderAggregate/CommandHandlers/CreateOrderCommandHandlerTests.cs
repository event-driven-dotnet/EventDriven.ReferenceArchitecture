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
    public class CreateOrderCommandHandlerTests
    {
        private readonly Mock<ILogger<CreateOrderCommandHandler>> _loggerMoq;
        private readonly IMapper _mapper;

        private readonly Mock<IOrderRepository> _repositoryMoq;

        public CreateOrderCommandHandlerTests()
        {
            _repositoryMoq = new Mock<IOrderRepository>();
            _loggerMoq = new Mock<ILogger<CreateOrderCommandHandler>>();
            _mapper = BaseUtils.GetMapper();
        }

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType()
        {
            var handler = new CreateOrderCommandHandler(_repositoryMoq.Object, _loggerMoq.Object);

            Assert.NotNull(handler);
            Assert.IsType<CreateOrderCommandHandler>(handler);
        }

        [Fact]
        public async Task WhenOrderDoesNotSaveCorrectly_ThenShouldReturnInvalidCommand()
        {
            _repositoryMoq.Setup(x => x.AddOrder(It.IsAny<Order>()))
                          .ReturnsAsync((Order) null);
            var handler = new CreateOrderCommandHandler(_repositoryMoq.Object, _loggerMoq.Object);

            var result = await handler.Handle(new CreateOrder(_mapper.Map<Order>(Orders.Order1)), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.InvalidCommand, result.Outcome);
        }

        [Fact]
        public async Task WhenOrderIsCreated_ThenShouldReturnEntity()
        {
            var order = _mapper.Map<Order>(Orders.Order1);
            _repositoryMoq.Setup(x => x.AddOrder(It.IsAny<Order>()))
                          .ReturnsAsync(order);
            var handler = new CreateOrderCommandHandler(_repositoryMoq.Object, _loggerMoq.Object);

            var result = await handler.Handle(new CreateOrder(order), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.Accepted, result.Outcome);
            Assert.Equal(order.Id, result.Entity.Id);
        }
    }
}