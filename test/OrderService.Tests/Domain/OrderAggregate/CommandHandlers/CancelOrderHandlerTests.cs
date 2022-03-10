using System;
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

public class CancelOrderHandlerTests
{
    private readonly Mock<ILogger<CancelOrderHandler>> _loggerMoq;
    private readonly IMapper _mapper;

    private readonly Mock<IOrderRepository> _repositoryMoq;

    public CancelOrderHandlerTests()
    {
        _repositoryMoq = new Mock<IOrderRepository>();
        _loggerMoq = new Mock<ILogger<CancelOrderHandler>>();
        _mapper = BaseUtils.GetMapper();
    }

    [Fact]
    public void WhenInstantiated_ThenShouldBeOfCorrectType()
    {
        var handler = new CancelOrderHandler(_repositoryMoq.Object, _loggerMoq.Object);

        Assert.NotNull(handler);
        Assert.IsType<CancelOrderHandler>(handler);
    }

    [Fact]
    public async Task WhenOrderDoesNotExist_ThenShouldReturnNotFound()
    {
        _repositoryMoq.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Order) null!);
        var handler = new CancelOrderHandler(_repositoryMoq.Object, _loggerMoq.Object);

        var result = await handler.Handle(new CancelOrder(new Order()), default);

        Assert.NotNull(result);
        Assert.Equal(CommandOutcome.NotFound, result.Outcome);
    }

    [Fact]
    public async Task WhenOrderIsCancelled_ThenResultingOrderStateShouldBeSetToCancelled()
    {
        var order = _mapper.Map<Order>(Orders.Order1);
        _repositoryMoq.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(order);
        _repositoryMoq.Setup(x => x.UpdateOrderStateAsync(It.IsAny<Order>(), It.IsAny<OrderState>()))
            .ReturnsAsync(order);
        var handler = new CancelOrderHandler(_repositoryMoq.Object, _loggerMoq.Object);

        var result = await handler.Handle(new CancelOrder(new Order()), default);

        Assert.NotNull(result);
        Assert.Equal(OrderState.Cancelled, result.Entity!.OrderState);
    }

    [Fact]
    public async Task WhenConcurrencyExceptionOccurs_ThenShouldReturnConflict()
    {
        var order = _mapper.Map<Order>(Orders.Order1);
        _repositoryMoq.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(order);
        _repositoryMoq.Setup(x => x.UpdateOrderStateAsync(It.IsAny<Order>(), It.IsAny<OrderState>()))
            .ThrowsAsync(new ConcurrencyException());
        var handler = new CancelOrderHandler(_repositoryMoq.Object, _loggerMoq.Object);

        var result = await handler.Handle(new CancelOrder(new Order()), default);

        Assert.NotNull(result);
        Assert.Equal(CommandOutcome.Conflict, result.Outcome);
    }
}