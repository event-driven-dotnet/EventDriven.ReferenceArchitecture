namespace OrderService.Tests.Controllers {

    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using OrderService.Domain.OrderAggregate;
    using OrderService.Domain.OrderAggregate.Commands;
    using EventDriven.CQRS.Abstractions.Commands;
    using Fakes;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using OrderService.Controllers;
    using Utils;
    using Xunit;

    public class OrderCommandControllerTests {

        private readonly IMapper mapper;
        private readonly Mock<ICommandBroker> commandBrokerMoq;

        public OrderCommandControllerTests() {
            mapper = BaseUtils.GetMapper();
            commandBrokerMoq = new Mock<ICommandBroker>();
        }

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType() {
            var controller = new OrderCommandController(commandBrokerMoq.Object, mapper);

            Assert.NotNull(controller);
            Assert.IsAssignableFrom<ControllerBase>(controller);
            Assert.IsType<OrderCommandController>(controller);
        }

        [Fact]
        public async Task GivenWeAreCreatingAnOrder_WhenSuccessful_ThenShouldProvideNewEntityWithPath() {
            var orderOut = mapper.Map<Order>(Orders.Order1);
            commandBrokerMoq.Setup(x => x.InvokeAsync<CreateOrder, CommandResult<Order>>(It.IsAny<CreateOrder>()))
                            .ReturnsAsync(new CommandResult<Order>(CommandOutcome.Accepted, orderOut));
            var controller = new OrderCommandController(commandBrokerMoq.Object, mapper);

            var actionResult = await controller.Create(Orders.Order1);
            var createdResult = actionResult as CreatedResult;

            Assert.NotNull(actionResult);
            Assert.NotNull(createdResult);
            Assert.Equal($"api/order/{orderOut.Id}", createdResult.Location, true);

        }

        [Fact]
        public async Task GivenWeAreCreatingAnOrder_WhenFailure_ThenShouldReturnError() {
            commandBrokerMoq.Setup(x => x.InvokeAsync<CreateOrder, CommandResult<Order>>(It.IsAny<CreateOrder>()))
                            .ReturnsAsync(new CommandResult<Order>(CommandOutcome.NotHandled));
            var controller = new OrderCommandController(commandBrokerMoq.Object, mapper);

            var actionResult = await controller.Create(Orders.Order1);
            var statusCodeResult = actionResult as StatusCodeResult;

            Assert.NotNull(actionResult);
            Assert.NotNull(statusCodeResult);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GivenWeAreUpdatingAnOrder_WhenSuccessful_ThenUpdatedEntityShouldBeReturned() {
            var orderIn = Orders.Order1;
            var orderOut = mapper.Map<Order>(orderIn);
            var controller = new OrderCommandController(commandBrokerMoq.Object, mapper);
            commandBrokerMoq.Setup(x => x.InvokeAsync<UpdateOrder, CommandResult<Order>>(It.IsAny<UpdateOrder>()))
                            .ReturnsAsync(new CommandResult<Order>(CommandOutcome.Accepted, orderOut));

            var actionResult = await controller.Update(orderIn);
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            
            Assert.NotNull(actionResult);
            Assert.NotNull(okResult);
            Assert.Equal(orderIn.Id, ((DTO.Write.Order) okResult.Value).Id);
        }

        [Fact]
        public async Task GivenWeAreUpdatingAnOrder_WhenOrderDoesNotExist_ThenShouldReturnNotFound() {
            commandBrokerMoq.Setup(x => x.InvokeAsync<UpdateOrder, CommandResult<Order>>(It.IsAny<UpdateOrder>()))
                            .ReturnsAsync(new CommandResult<Order>(CommandOutcome.NotFound));
            var controller = new OrderCommandController(commandBrokerMoq.Object, mapper);

            var actionResult = await controller.Update(Orders.Order1);
            var notFoundResult = actionResult as NotFoundResult;

            Assert.NotNull(actionResult);
            Assert.NotNull(notFoundResult);
        }
        
        [Fact]
        public async Task GivenWeAreUpdatingAnOrder_WhenWeEncounterAConcurrencyIssue_ThenShouldReturnConflict() {
            commandBrokerMoq.Setup(x => x.InvokeAsync<UpdateOrder, CommandResult<Order>>(It.IsAny<UpdateOrder>()))
                            .ReturnsAsync(new CommandResult<Order>(CommandOutcome.Conflict));
            var controller = new OrderCommandController(commandBrokerMoq.Object, mapper);

            var actionResult = await controller.Update(Orders.Order1);
            var conflictResult = actionResult as ConflictResult;

            Assert.NotNull(actionResult);
            Assert.NotNull(conflictResult);
        }
        
        [Fact]
        public async Task GivenWeAreRemovingAnOrder_WhenSuccessful_ThenShouldReturnSuccess() {
            var orderId = Guid.NewGuid();
            commandBrokerMoq.Setup(x => x.InvokeAsync<RemoveOrder, CommandResult>(It.IsAny<RemoveOrder>()))
                            .ReturnsAsync(new CommandResult(CommandOutcome.Accepted));
            var controller = new OrderCommandController(commandBrokerMoq.Object, mapper);

            var actionResult = await controller.Remove(orderId);
            var noContentResult = actionResult as NoContentResult;
            
            Assert.NotNull(actionResult);
            Assert.NotNull(noContentResult);
        }

        [Fact]
        public async Task GivenWeAreShippingAnOrder_WhenSuccessful_ThenShouldReturnEntity() {
            var orderOut = mapper.Map<Order>(Orders.Order2);
            commandBrokerMoq.Setup(x => x.InvokeAsync<ShipOrder, CommandResult<Order>>(It.IsAny<ShipOrder>()))
                            .ReturnsAsync(new CommandResult<Order>(CommandOutcome.Accepted, orderOut));
            var controller = new OrderCommandController(commandBrokerMoq.Object, mapper);

            var actionResult = await controller.Ship(orderOut.Id, orderOut.ETag);
            var okResult = Assert.IsType<OkObjectResult>(actionResult);

            Assert.NotNull(actionResult);
            Assert.NotNull(okResult);
            Assert.Equal(orderOut.Id, ((DTO.Write.Order) okResult.Value).Id);
        }
        
        [Fact]
        public async Task GivenWeAreShippingAnOrder_WhenOrderDoesNotExist_ThenShouldReturnNotFound() {
            var orderOut = mapper.Map<Order>(Orders.Order2);
            commandBrokerMoq.Setup(x => x.InvokeAsync<ShipOrder, CommandResult<Order>>(It.IsAny<ShipOrder>()))
                            .ReturnsAsync(new CommandResult<Order>(CommandOutcome.NotFound));
            var controller = new OrderCommandController(commandBrokerMoq.Object, mapper);

            var actionResult = await controller.Ship(orderOut.Id, orderOut.ETag);
            var notFoundResult = actionResult as NotFoundResult;

            Assert.NotNull(actionResult);
            Assert.NotNull(notFoundResult);
        }
        
        [Fact]
        public async Task GivenWeAreCancellingAnOrder_WhenSuccessful_ThenShouldReturnEntity() {
            var orderOut = mapper.Map<Order>(Orders.Order2);
            commandBrokerMoq.Setup(x => x.InvokeAsync<CancelOrder, CommandResult<Order>>(It.IsAny<CancelOrder>()))
                            .ReturnsAsync(new CommandResult<Order>(CommandOutcome.Accepted, orderOut));
            var controller = new OrderCommandController(commandBrokerMoq.Object, mapper);

            var actionResult = await controller.Cancel(orderOut.Id, orderOut.ETag);
            var okResult = Assert.IsType<OkObjectResult>(actionResult);

            Assert.NotNull(actionResult);
            Assert.NotNull(okResult);
            Assert.Equal(orderOut.Id, ((DTO.Write.Order) okResult.Value).Id);
        }
        
        [Fact]
        public async Task GivenWeAreCancellingAnOrder_WhenOrderDoesNotExist_ThenShouldReturnNotFound() {
            var orderOut = mapper.Map<Order>(Orders.Order2);
            commandBrokerMoq.Setup(x => x.InvokeAsync<CancelOrder, CommandResult<Order>>(It.IsAny<CancelOrder>()))
                            .ReturnsAsync(new CommandResult<Order>(CommandOutcome.NotFound));
            var controller = new OrderCommandController(commandBrokerMoq.Object, mapper);

            var actionResult = await controller.Cancel(orderOut.Id, orderOut.ETag);
            var notFoundResult = actionResult as NotFoundResult;

            Assert.NotNull(actionResult);
            Assert.NotNull(notFoundResult);
        }

    }

}