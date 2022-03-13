using System.Threading.Tasks;
using AutoMapper;
using EventDriven.DDD.Abstractions.Commands;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Domain.OrderAggregate;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Domain.OrderAggregate.Handlers;
using OrderService.Repositories;
using OrderService.Tests.Fakes;
using OrderService.Tests.Helpers;
using Xunit;

namespace OrderService.Tests.Domain.OrderAggregate.CommandHandlers;

public class CreateOrderHandlerTests
{
    private readonly Mock<ILogger<CreateOrderHandler>> _loggerMoq;
    private readonly IMapper _mapper;

    private readonly Mock<IOrderRepository> _repositoryMoq;

    public CreateOrderHandlerTests()
    {
        _repositoryMoq = new Mock<IOrderRepository>();
        _loggerMoq = new Mock<ILogger<CreateOrderHandler>>();
        _mapper = MappingHelper.GetMapper();
    }

    [Fact]
    public void WhenInstantiated_ThenShouldBeOfCorrectType()
    {
        var handler = new CreateOrderHandler(_repositoryMoq.Object, _loggerMoq.Object);

        Assert.NotNull(handler);
        Assert.IsType<CreateOrderHandler>(handler);
    }

    [Fact]
    public async Task WhenOrderDoesNotSaveCorrectly_ThenShouldReturnInvalidCommand()
    {
        _repositoryMoq.Setup(x => x.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order) null!);
        var handler = new CreateOrderHandler(_repositoryMoq.Object, _loggerMoq.Object);

        var result = await handler.Handle(new CreateOrder(_mapper.Map<Order>(Orders.Order1)), default);

        Assert.NotNull(result);
        Assert.Equal(CommandOutcome.InvalidCommand, result.Outcome);
    }

    [Fact]
    public async Task WhenOrderIsCreated_ThenShouldReturnEntity()
    {
        var order = _mapper.Map<Order>(Orders.Order1);
        _repositoryMoq.Setup(x => x.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync(order);
        var handler = new CreateOrderHandler(_repositoryMoq.Object, _loggerMoq.Object);

        var result = await handler.Handle(new CreateOrder(order), default);

        Assert.NotNull(result);
        Assert.Equal(CommandOutcome.Accepted, result.Outcome);
        Assert.Equal(order.Id, result.Entity!.Id);
    }
}