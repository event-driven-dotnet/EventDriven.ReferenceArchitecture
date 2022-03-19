using System.Collections.Generic;
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

public class GetOrdersHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IOrderRepository> _repositoryMock;

    public GetOrdersHandlerTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _mapper = MappingHelper.GetMapper();
    }
    
    [Fact]
    public void WhenInstantiated_ThenShouldBeOfCorrectType()
    {
        var handler = new GetOrdersHandler(_repositoryMock.Object);

        Assert.NotNull(handler);
        Assert.IsType<GetOrdersHandler>(handler);
    }

    [Fact]
    public async Task WhenRetrievingEntities_ThenAllEntitiesShouldBeReturned()
    {
        _repositoryMock.Setup(x => x.GetAsync())
            .ReturnsAsync(new List<Order>
            {
                _mapper.Map<Order>(Orders.Order1),                    
                _mapper.Map<Order>(Orders.Order2)
            });

        var handler = new GetOrdersHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetOrders(), default);

        Assert.Collection(result,
            c => Assert.Equal(OrderViews.Order1.Id, c.Id),
            c => Assert.Equal(OrderViews.Order2.Id, c.Id));
    }
}