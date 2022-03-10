using System.Threading.Tasks;
using AutoMapper;
using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Domain.OrderAggregate;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Domain.OrderAggregate.Handlers;
using OrderService.Repositories;
using OrderService.Tests.Fakes;
using OrderService.Tests.Utils;
using Xunit;

namespace OrderService.Tests.Domain.OrderAggregate.CommandHandlers;

public class UpdateOrderHandlerTests
{
    private readonly Mock<ILogger<UpdateOrderHandler>> _loggerMoq;
    private readonly IMapper _mapper;

    private readonly Mock<IOrderRepository> _repositoryMoq;

    public UpdateOrderHandlerTests()
    {
        _repositoryMoq = new Mock<IOrderRepository>();
        _loggerMoq = new Mock<ILogger<UpdateOrderHandler>>();
        _mapper = BaseUtils.GetMapper();
    }

    [Fact]
    public void WhenInstantiated_ThenShouldBeOfCorrectType()
    {
        var handler = new UpdateOrderHandler(_repositoryMoq.Object, _loggerMoq.Object);

        Assert.NotNull(handler);
        Assert.IsType<UpdateOrderHandler>(handler);
    }

    [Fact]
    public async Task WhenOrderFailsToUpdate_ThenShouldReturnNotFound()
    {
        _repositoryMoq.Setup(x => x.UpdateAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order) null!);
        var handler = new UpdateOrderHandler(_repositoryMoq.Object, _loggerMoq.Object);

        var result = await handler.Handle(new UpdateOrder(new Order()), default);

        Assert.NotNull(result);
        Assert.Equal(CommandOutcome.NotFound, result.Outcome);
    }

    [Fact]
    public async Task WhenConcurrencyExceptionOccurs_ThenShouldReturnConflict()
    {
        _repositoryMoq.Setup(x => x.UpdateAsync(It.IsAny<Order>()))
            .ThrowsAsync(new ConcurrencyException());
        var handler = new UpdateOrderHandler(_repositoryMoq.Object, _loggerMoq.Object);

        var result = await handler.Handle(new UpdateOrder(new Order()), default);

        Assert.NotNull(result);
        Assert.Equal(CommandOutcome.Conflict, result.Outcome);
    }

    [Fact]
    public async Task WhenOrderIsUpdated_ThenShouldReturnAcceptedEntity()
    {
        var order = _mapper.Map<Order>(Orders.Order1);
        _repositoryMoq.Setup(x => x.UpdateAsync(It.IsAny<Order>()))
            .ReturnsAsync(order);
        var handler = new UpdateOrderHandler(_repositoryMoq.Object, _loggerMoq.Object);

        var result = await handler.Handle(new UpdateOrder(order), default);

        Assert.NotNull(result);
        Assert.Equal(CommandOutcome.Accepted, result.Outcome);
        Assert.Equal(order.Id, result.Entity!.Id);
    }
}