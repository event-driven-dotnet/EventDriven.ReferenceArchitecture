using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderService.Controllers;
using OrderService.Domain.OrderAggregate;
using OrderService.DTO.Read;
using OrderService.Repositories;
using OrderService.Tests.Fakes;
using OrderService.Tests.Utils;
using Xunit;

namespace OrderService.Tests.Controllers;

public class OrderQueryControllerTests
{
    private readonly IMapper _mapper;

    private readonly Mock<IOrderRepository> _repositoryMoq;

    public OrderQueryControllerTests()
    {
        _repositoryMoq = new Mock<IOrderRepository>();
        _mapper = BaseUtils.GetMapper();
    }

    [Fact]
    public void WhenInstantiated_ThenShouldBeOfCorrectType()
    {
        var controller = new OrderQueryController(_repositoryMoq.Object);

        Assert.NotNull(controller);
        Assert.IsAssignableFrom<ControllerBase>(controller);
        Assert.IsType<OrderQueryController>(controller);
    }

    [Fact]
    public async Task WhenRetrievingAllOrders_ThenAllOrdersShouldBeReturned()
    {
        _repositoryMoq.Setup(x => x.GetAsync())
            .ReturnsAsync(new List<Order>
            {
                _mapper.Map<Order>(Orders.Order1),
                _mapper.Map<Order>(Orders.Order2)
            });
        var controller = new OrderQueryController(_repositoryMoq.Object);

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
        _repositoryMoq.Setup(x => x.GetByCustomerAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<Order>
            {
                _mapper.Map<Order>(Orders.Order1),
                _mapper.Map<Order>(Orders.Order2)
            });
        var controller = new OrderQueryController(_repositoryMoq.Object);

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
        _repositoryMoq.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_mapper.Map<Order>(Orders.Order1));
        var controller = new OrderQueryController(_repositoryMoq.Object);

        var actionResult = await controller.GetOrder(Guid.NewGuid());
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var value = (OrderView) okResult.Value!;

        Assert.Equal(OrderViews.Order1.Id, value.Id);
    }
}