using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventDriven.CQRS.Abstractions.Commands;
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
    public class RemoveOrderHandlerTests
    {
        private readonly Mock<IOrderRepository> _repositoryMoq;
        private readonly IMapper _mapper;

        public RemoveOrderHandlerTests()
        {
            _repositoryMoq = new Mock<IOrderRepository>();
            _mapper = MappingHelper.GetMapper();
        }

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType()
        {
            var handler = new RemoveOrderHandler(_repositoryMoq.Object);

            Assert.NotNull(handler);
            Assert.IsType<RemoveOrderHandler>(handler);
        }

        [Fact]
        public async Task WhenRemoved_ThenShouldReturnAccepted()
        {
            _repositoryMoq.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(_mapper.Map<Order>(Orders.Order1));
            var handler = new RemoveOrderHandler(_repositoryMoq.Object);

            var result = await handler.Handle(new RemoveOrder(Guid.Empty), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(CommandOutcome.Accepted, result.Outcome);
        }
    }
}