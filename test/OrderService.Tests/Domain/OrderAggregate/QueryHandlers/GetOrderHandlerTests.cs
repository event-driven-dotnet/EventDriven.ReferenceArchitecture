using System;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using OrderService.Domain.OrderAggregate;
using OrderService.Domain.OrderAggregate.Queries;
using OrderService.Domain.OrderAggregate.QueryHandlers;
using OrderService.Repositories;
using OrderService.Tests.Fakes;
using OrderService.Tests.Helpers;
using Xunit;

namespace OrderService.Tests.Domain.OrderAggregate.QueryHandlers;

public class GetOrderHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IOrderRepository> _repositoryMock;

    public GetOrderHandlerTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _mapper = MappingHelper.GetMapper();
    }
    
    [Fact]
    public void WhenInstantiated_ThenShouldBeOfCorrectType()
    {
        var handler = new GetOrderHandler(_repositoryMock.Object);

        Assert.NotNull(handler);
        Assert.IsType<GetOrderHandler>(handler);
    }

    [Fact]
    public async Task WhenRetrievingEntity_ThenEntityShouldBeReturned()
    {
        var expected = _mapper.Map<Order>(Orders.Order1);
        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expected);

        var handler = new GetOrderHandler(_repositoryMock.Object);

        var actual = await handler.Handle(new GetOrder(Guid.Empty), default);

        Assert.Equal(expected, actual);
    }
}