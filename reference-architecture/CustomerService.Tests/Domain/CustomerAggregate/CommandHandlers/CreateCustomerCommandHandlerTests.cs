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

namespace CustomerService.Tests.Domain.CustomerAggregate.CommandHandlers
{

    public class CreateCustomerCommandHandlerTests
    {

        private readonly Mock<ILogger<CreateCustomerCommandHandler>> _loggerMoq;
        private readonly Mock<ICustomerRepository> _repositoryMoq;

        public CreateCustomerCommandHandlerTests()
        {
            _loggerMoq = new Mock<ILogger<CreateCustomerCommandHandler>>();
            _repositoryMoq = new Mock<ICustomerRepository>();
        }

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType()
        {
            var handler = new CreateCustomerCommandHandler(_loggerMoq.Object, _repositoryMoq.Object);

            Assert.NotNull(handler);
            Assert.IsType<CreateCustomerCommandHandler>(handler);
        }

        [Fact]
        public async Task WhenCreatingEntityFails_ThenShouldReturnFailure()
        {
            var handler = new CreateCustomerCommandHandler(_loggerMoq.Object, _repositoryMoq.Object);

            var cmdResult = await handler.Handle(new CreateCustomer(new Customer()), CancellationToken.None);

            Assert.Equal(CommandOutcome.InvalidCommand, cmdResult.Outcome);
        }

        [Fact]
        public async Task WhenEntityIsCreated_ThenShouldReturnSuccess()
        {
            var customer = new Customer();
            _repositoryMoq.Setup(x => x.Add(It.IsAny<Customer>()))
                          .ReturnsAsync(customer);
            var handler = new CreateCustomerCommandHandler(_loggerMoq.Object, _repositoryMoq.Object);

            var cmdResult = await handler.Handle(new CreateCustomer(customer), CancellationToken.None);

            Assert.Equal(CommandOutcome.Accepted, cmdResult.Outcome);
        }

        [Fact]
        public async Task WhenEntityIsCreated_ThenShouldReturnNewEntity()
        {
            var customer = new Customer();
            _repositoryMoq.Setup(x => x.Add(It.IsAny<Customer>()))
                          .ReturnsAsync(customer);
            var handler = new CreateCustomerCommandHandler(_loggerMoq.Object, _repositoryMoq.Object);

            var cmdResult = await handler.Handle(new CreateCustomer(customer), CancellationToken.None);

            Assert.Equal(customer.Id,
                cmdResult.Entities[0]
                         .Id);
        }

    }

}