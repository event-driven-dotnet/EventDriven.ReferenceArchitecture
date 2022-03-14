using System;
using System.Threading.Tasks;
using AutoMapper;
using EventDriven.CQRS.Abstractions.Commands;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderService.Controllers;
using OrderService.Domain.OrderAggregate;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Tests.Fakes;
using OrderService.Tests.Helpers;
using Xunit;

namespace OrderService.Tests.Controllers;

public class OrderCommandControllerTests
{
    private readonly Mock<ICommandBroker> _commandBrokerMoq;
    private readonly IMapper _mapper;

    public OrderCommandControllerTests()
    {
        _mapper = MappingHelper.GetMapper();
        _commandBrokerMoq = new Mock<ICommandBroker>();
    }

    [Fact]
    public void WhenInstantiated_ThenShouldBeOfCorrectType()
    {
        var controller = new OrderCommandController(_commandBrokerMoq.Object, _mapper);

        Assert.NotNull(controller);
        Assert.IsAssignableFrom<ControllerBase>(controller);
        Assert.IsType<OrderCommandController>(controller);
    }

    [Fact]
    public async Task GivenWeAreCreatingAnOrder_WhenSuccessful_ThenShouldProvideNewEntityWithPath()
    {
        var orderOut = _mapper.Map<Order>(Orders.Order1);
        _commandBrokerMoq.Setup(x => x.SendAsync(It.IsAny<CreateOrder>()))
            .ReturnsAsync(new CommandResult<Order>(CommandOutcome.Accepted, orderOut));
        var controller = new OrderCommandController(_commandBrokerMoq.Object, _mapper);

        var actionResult = await controller.Create(Orders.Order1);
        var createdResult = actionResult as CreatedResult;

        Assert.NotNull(actionResult);
        Assert.NotNull(createdResult);
        Assert.Equal($"api/order/{orderOut.Id}", createdResult!.Location, true);
    }

    [Fact]
    public async Task GivenWeAreCreatingAnOrder_WhenFailure_ThenShouldReturnError()
    {
        _commandBrokerMoq.Setup(x => x.SendAsync(It.IsAny<CreateOrder>()))
            .ReturnsAsync(new CommandResult<Order>(CommandOutcome.NotHandled));
        var controller = new OrderCommandController(_commandBrokerMoq.Object, _mapper);

        var actionResult = await controller.Create(Orders.Order1);
        var statusCodeResult = actionResult as StatusCodeResult;

        Assert.NotNull(actionResult);
        Assert.NotNull(statusCodeResult);
        Assert.Equal(500, statusCodeResult!.StatusCode);
    }

    [Fact]
    public async Task GivenWeAreUpdatingAnOrder_WhenSuccessful_ThenUpdatedEntityShouldBeReturned()
    {
        var orderIn = Orders.Order1;
        var orderOut = _mapper.Map<Order>(orderIn);
        var controller = new OrderCommandController(_commandBrokerMoq.Object, _mapper);
        _commandBrokerMoq.Setup(x => x.SendAsync(It.IsAny<UpdateOrder>()))
            .ReturnsAsync(new CommandResult<Order>(CommandOutcome.Accepted, orderOut));

        var actionResult = await controller.Update(orderIn);
        var okResult = Assert.IsType<OkObjectResult>(actionResult);

        Assert.NotNull(actionResult);
        Assert.NotNull(okResult);
        Assert.Equal(orderIn.Id, ((DTO.Write.Order) okResult.Value!).Id);
    }

    [Fact]
    public async Task GivenWeAreUpdatingAnOrder_WhenOrderDoesNotExist_ThenShouldReturnNotFound()
    {
        _commandBrokerMoq.Setup(x => x.SendAsync(It.IsAny<UpdateOrder>()))
            .ReturnsAsync(new CommandResult<Order>(CommandOutcome.NotFound));
        var controller = new OrderCommandController(_commandBrokerMoq.Object, _mapper);

        var actionResult = await controller.Update(Orders.Order1);
        var notFoundResult = actionResult as NotFoundResult;

        Assert.NotNull(actionResult);
        Assert.NotNull(notFoundResult);
    }

    [Fact]
    public async Task GivenWeAreUpdatingAnOrder_WhenWeEncounterAConcurrencyIssue_ThenShouldReturnConflict()
    {
        _commandBrokerMoq.Setup(x => x.SendAsync(It.IsAny<UpdateOrder>()))
            .ReturnsAsync(new CommandResult<Order>(CommandOutcome.Conflict));
        var controller = new OrderCommandController(_commandBrokerMoq.Object, _mapper);

        var actionResult = await controller.Update(Orders.Order1);
        var conflictResult = actionResult as ConflictResult;

        Assert.NotNull(actionResult);
        Assert.NotNull(conflictResult);
    }

    [Fact]
    public async Task GivenWeAreRemovingAnOrder_WhenSuccessful_ThenShouldReturnSuccess()
    {
        var orderId = Guid.NewGuid();
        _commandBrokerMoq.Setup(x => x.SendAsync(It.IsAny<RemoveOrder>()))
            .ReturnsAsync(new CommandResult(CommandOutcome.Accepted));
        var controller = new OrderCommandController(_commandBrokerMoq.Object, _mapper);

        var actionResult = await controller.Remove(orderId);
        var noContentResult = actionResult as NoContentResult;

        Assert.NotNull(actionResult);
        Assert.NotNull(noContentResult);
    }

    [Fact]
    public async Task GivenWeAreShippingAnOrder_WhenSuccessful_ThenShouldReturnEntity()
    {
        var orderIn = Orders.Order1;
        var orderOut = _mapper.Map<Order>(Orders.Order2);
        var controller = new OrderCommandController(_commandBrokerMoq.Object, _mapper);
        _commandBrokerMoq.Setup(x => x.SendAsync(It.IsAny<ShipOrder>()))
            .ReturnsAsync(new CommandResult<Order>(CommandOutcome.Accepted, orderOut));

        var actionResult = await controller.Ship(orderIn.Id);
        var okResult = Assert.IsType<OkObjectResult>(actionResult);

        Assert.NotNull(actionResult);
        Assert.NotNull(okResult);
        Assert.Equal(orderOut.Id, ((DTO.Write.Order) okResult.Value!).Id);
    }

    [Fact]
    public async Task GivenWeAreShippingAnOrder_WhenOrderDoesNotExist_ThenShouldReturnNotFound()
    {
        _commandBrokerMoq.Setup(x => x.SendAsync(It.IsAny<ShipOrder>()))
            .ReturnsAsync(new CommandResult<Order>(CommandOutcome.NotFound));
        var controller = new OrderCommandController(_commandBrokerMoq.Object, _mapper);

        var actionResult = await controller.Ship(Orders.Order2.Id);
        var notFoundResult = actionResult as NotFoundResult;

        Assert.NotNull(actionResult);
        Assert.NotNull(notFoundResult);
    }

    [Fact]
    public async Task GivenWeAreCancellingAnOrder_WhenSuccessful_ThenShouldReturnEntity()
    {
        var orderIn = Orders.Order2;
        var orderOut = _mapper.Map<Order>(Orders.Order2);
        var controller = new OrderCommandController(_commandBrokerMoq.Object, _mapper);
        _commandBrokerMoq.Setup(x => x.SendAsync(It.IsAny<CancelOrder>()))
            .ReturnsAsync(new CommandResult<Order>(CommandOutcome.Accepted, orderOut));

        var actionResult = await controller.Cancel(orderIn.Id);
        var okResult = Assert.IsType<OkObjectResult>(actionResult);

        Assert.NotNull(actionResult);
        Assert.NotNull(okResult);
        Assert.Equal(orderOut.Id, ((DTO.Write.Order) okResult.Value!).Id);
    }

    [Fact]
    public async Task GivenWeAreCancellingAnOrder_WhenOrderDoesNotExist_ThenShouldReturnNotFound()
    {
        _commandBrokerMoq.Setup(x => x.SendAsync(It.IsAny<CancelOrder>()))
            .ReturnsAsync(new CommandResult<Order>(CommandOutcome.NotFound));
        var controller = new OrderCommandController(_commandBrokerMoq.Object, _mapper);

        var actionResult = await controller.Cancel(Orders.Order2.Id);
        var notFoundResult = actionResult as NotFoundResult;

        Assert.NotNull(actionResult);
        Assert.NotNull(notFoundResult);
    }
}