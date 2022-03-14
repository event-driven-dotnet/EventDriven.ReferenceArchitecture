using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EventDriven.CQRS.Abstractions.Queries;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderService.Controllers;
using OrderService.Domain.OrderAggregate;
using OrderService.Domain.OrderAggregate.Queries;
using OrderService.DTO.Read;
using OrderService.Tests.Fakes;
using OrderService.Tests.Helpers;
using Xunit;

namespace OrderService.Tests.Controllers;

public class OrderQueryControllerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IQueryBroker> _queryBrokerMoq;

    public OrderQueryControllerTests()
    {
        _queryBrokerMoq = new Mock<IQueryBroker>();
        _mapper = MappingHelper.GetMapper();
    }

    [Fact]
    public void WhenInstantiated_ThenShouldBeOfCorrectType()
    {
        var controller = new OrderQueryController(_queryBrokerMoq.Object);

        Assert.NotNull(controller);
        Assert.IsAssignableFrom<ControllerBase>(controller);
        Assert.IsType<OrderQueryController>(controller);
    }

    [Fact]
    public async Task WhenRetrievingAllOrders_ThenAllOrdersShouldBeReturned()
    {
        _queryBrokerMoq.Setup(x => x.SendAsync(It.IsAny<GetOrders>()))
            .ReturnsAsync(new List<Order>
            {
                _mapper.Map<Order>(Orders.Order1),
                _mapper.Map<Order>(Orders.Order2)
            });
        var controller = new OrderQueryController(_queryBrokerMoq.Object);

        var actionResult = await controller.GetOrders();
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var value = (IEnumerable<OrderView>) okResult.Value!;

        Assert.Collection(value,
            x => Assert.Equal(OrderViews.Order1.Id, x.Id),
            x => Assert.Equal(OrderViews.Order2.Id, x.Id));
    }

    [Fact]
    public async Task WhenRetrievingAllOrdersForACustomer_ThenAllOrdersShouldBeReturned()
    {
        _queryBrokerMoq.Setup(x => x.SendAsync(It.IsAny<GetOrdersByCustomer>()))
            .ReturnsAsync(new List<Order>
            {
                _mapper.Map<Order>(Orders.Order1),
                _mapper.Map<Order>(Orders.Order2)
            });
        var controller = new OrderQueryController(_queryBrokerMoq.Object);

        var actionResult = await controller.GetOrders(Guid.NewGuid());
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var value = (IEnumerable<OrderView>) okResult.Value!;

        Assert.Collection(value,
            x => Assert.Equal(OrderViews.Order1.Id, x.Id),
            x => Assert.Equal(OrderViews.Order2.Id, x.Id));
    }

    [Fact]
    public async Task WhenRetrievingAnOrderById_ThenShouldReturnOrder()
    {
        _queryBrokerMoq.Setup(x => x.SendAsync(It.IsAny<GetOrder>()))
            .ReturnsAsync(_mapper.Map<Order>(Orders.Order1));
        var controller = new OrderQueryController(_queryBrokerMoq.Object);

        var actionResult = await controller.GetOrder(Guid.NewGuid());
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var value = (OrderView) okResult.Value!;

        Assert.Equal(OrderViews.Order1.Id, value.Id);
    }
}