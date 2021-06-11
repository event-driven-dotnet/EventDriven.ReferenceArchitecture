namespace CustomerService.Tests.Controllers {

    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using CustomerService.Controllers;
    using CustomerService.Domain.CustomerAggregate;
    using CustomerService.Domain.CustomerAggregate.Commands;
    using EventDriven.CQRS.Abstractions.Commands;
    using Fakes;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Utils;
    using Xunit;

    public class CustomerCommandControllerTests {

        private readonly Mock<ICommandBroker> commandBrokerMoq;
        private readonly IMapper mapper;

        public CustomerCommandControllerTests() {
            commandBrokerMoq = new Mock<ICommandBroker>();
            mapper = BaseUtils.GetMapper();
        }

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType() {
            var controller = new CustomerCommandController(commandBrokerMoq.Object, mapper);

            Assert.IsAssignableFrom<ControllerBase>(controller);
            Assert.IsType<CustomerCommandController>(controller);
        }

        [Fact]
        public async Task GivenWeAreCreatingACustomer_WhenSuccessful_ThenShouldProvideNewEntityWithPath() {
            var customerOut = mapper.Map<Customer>(Customers.Customer1);

            commandBrokerMoq.Setup(x => x.InvokeAsync<CreateCustomer, CommandResult<Customer>>(It.IsAny<CreateCustomer>()))
                            .ReturnsAsync(new CommandResult<Customer>(CommandOutcome.Accepted, customerOut));

            var controller = new CustomerCommandController(commandBrokerMoq.Object, mapper);

            var actionResult = await controller.Create(Customers.Customer1);
            var createdResult = actionResult as CreatedResult;

            Assert.NotNull(actionResult);
            Assert.NotNull(createdResult);
            Assert.Equal($"api/customer/{customerOut.Id}", createdResult.Location, true);
        }

        [Fact]
        public async Task GivenWeAreCreatingACustomer_WhenFailure_ThenShouldReturnError() {
            commandBrokerMoq.Setup(x => x.InvokeAsync<CreateCustomer, CommandResult<Customer>>(It.IsAny<CreateCustomer>()))
                            .ReturnsAsync(new CommandResult<Customer>(CommandOutcome.NotHandled));

            var controller = new CustomerCommandController(commandBrokerMoq.Object, mapper);

            var actionResult = await controller.Create(Customers.Customer1);
            var statusCodeResult = actionResult as StatusCodeResult;

            Assert.NotNull(actionResult);
            Assert.NotNull(statusCodeResult);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GivenWeAreUpdatingACustomer_WhenSuccessful_ThenUpdatedEntityShouldBeReturned() {
            var customerIn = Customers.Customer2;
            var customerOut = mapper.Map<Customer>(Customers.Customer2);

            var controller = new CustomerCommandController(commandBrokerMoq.Object, mapper);

            commandBrokerMoq.Setup(x => x.InvokeAsync<UpdateCustomer, CommandResult<Customer>>(It.IsAny<UpdateCustomer>()))
                            .ReturnsAsync(new CommandResult<Customer>(CommandOutcome.Accepted, customerOut));

            var actionResult = await controller.Update(customerIn);
            var objectResult = actionResult as OkObjectResult;

            Assert.NotNull(actionResult);
            Assert.NotNull(objectResult);
            Assert.Equal(customerIn.Id, ((DTO.Write.Customer) objectResult.Value).Id);
        }

        [Fact]
        public async Task GivenWeAreUpdatingACustomer_WhenCustomerDoesNotExist_ThenShouldReturnNotFound() {
            var controller = new CustomerCommandController(commandBrokerMoq.Object, mapper);

            commandBrokerMoq.Setup(x => x.InvokeAsync<UpdateCustomer, CommandResult<Customer>>(It.IsAny<UpdateCustomer>()))
                            .ReturnsAsync(new CommandResult<Customer>(CommandOutcome.NotFound));

            var actionResult = await controller.Update(Customers.Customer2);
            var notFoundResult = actionResult as NotFoundResult;

            Assert.NotNull(actionResult);
            Assert.NotNull(notFoundResult);
        }

        [Fact]
        public async Task GivenWeAreUpdatingACustomer_WhenWeEncounterAConcurrencyIssue_ThenShouldReturnConflict() {
            var controller = new CustomerCommandController(commandBrokerMoq.Object, mapper);

            commandBrokerMoq.Setup(x => x.InvokeAsync<UpdateCustomer, CommandResult<Customer>>(It.IsAny<UpdateCustomer>()))
                            .ReturnsAsync(new CommandResult<Customer>(CommandOutcome.Conflict));

            var actionResult = await controller.Update(Customers.Customer2);
            var conflictResult = actionResult as ConflictResult;

            Assert.NotNull(actionResult);
            Assert.NotNull(conflictResult);
        }

        [Fact]
        public async Task GivenWeAreRemovingACustomer_WhenSuccessful_ThenShouldReturnSuccess() {
            var customerId = Guid.NewGuid();
            var controller = new CustomerCommandController(commandBrokerMoq.Object, mapper);

            commandBrokerMoq.Setup(x => x.InvokeAsync<RemoveCustomer, CommandResult>(It.IsAny<RemoveCustomer>()))
                            .ReturnsAsync(new CommandResult<Customer>(CommandOutcome.Accepted));

            var actionResult = await controller.Remove(customerId);
            var noContentResult = actionResult as NoContentResult;

            Assert.NotNull(actionResult);
            Assert.NotNull(noContentResult);
        }

    }

}