namespace CustomerService.Tests.Domain.CustomerAggregate.CommandHandlers {

    using System.Threading;
    using System.Threading.Tasks;
    using CustomerService.Domain.CustomerAggregate;
    using CustomerService.Domain.CustomerAggregate.CommandHandlers;
    using CustomerService.Domain.CustomerAggregate.Commands;
    using CustomerService.Repositories;
    using EventDriven.CQRS.Abstractions.Commands;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class CreateCustomerCommandHandlerTests {

        private readonly Mock<ILogger<CreateCustomerCommandHandler>> loggerMoq;
        private readonly Mock<ICustomerRepository> repositoryMoq;

        public CreateCustomerCommandHandlerTests() {
            loggerMoq = new Mock<ILogger<CreateCustomerCommandHandler>>();
            repositoryMoq = new Mock<ICustomerRepository>();
        }

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType() {
            var handler = new CreateCustomerCommandHandler(loggerMoq.Object, repositoryMoq.Object);

            Assert.NotNull(handler);
            Assert.IsType<CreateCustomerCommandHandler>(handler);
        }

        [Fact]
        public async Task WhenCreatingEntityFails_ThenShouldReturnFailure() {
            var handler = new CreateCustomerCommandHandler(loggerMoq.Object, repositoryMoq.Object);

            var cmdResult = await handler.Handle(new CreateCustomer(new Customer()), CancellationToken.None);

            Assert.Equal(CommandOutcome.InvalidCommand, cmdResult.Outcome);
        }

        [Fact]
        public async Task WhenEntityIsCreated_ThenShouldReturnSuccess() {
            var customer = new Customer();
            repositoryMoq.Setup(x => x.Add(It.IsAny<Customer>()))
                         .ReturnsAsync(customer);
            var handler = new CreateCustomerCommandHandler(loggerMoq.Object, repositoryMoq.Object);

            var cmdResult = await handler.Handle(new CreateCustomer(customer), CancellationToken.None);

            Assert.Equal(CommandOutcome.Accepted, cmdResult.Outcome);
        }

        [Fact]
        public async Task WhenEntityIsCreated_ThenShouldReturnNewEntity() {
            var customer = new Customer();
            repositoryMoq.Setup(x => x.Add(It.IsAny<Customer>()))
                         .ReturnsAsync(customer);
            var handler = new CreateCustomerCommandHandler(loggerMoq.Object, repositoryMoq.Object);

            var cmdResult = await handler.Handle(new CreateCustomer(customer), CancellationToken.None);

            Assert.Equal(customer.Id, cmdResult.Entities[0]
                                               .Id);
        }

    }

}