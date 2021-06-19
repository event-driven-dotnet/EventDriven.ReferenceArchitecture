using System;
using System.Threading;
using System.Threading.Tasks;
using CustomerService.Domain.CustomerAggregate.CommandHandlers;
using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Repositories;
using EventDriven.CQRS.Abstractions.Commands;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CustomerService.Tests.Domain.CustomerAggregate.CommandHandlers
{

    public class RemoveCustomerCommandHandlerTests
    {

        private readonly Mock<ILogger<RemoveCustomerCommandHandler>> _loggerMoq;

        private readonly Mock<ICustomerRepository> _repositoryMoq;

        public RemoveCustomerCommandHandlerTests()
        {
            _loggerMoq = new Mock<ILogger<RemoveCustomerCommandHandler>>();
            _repositoryMoq = new Mock<ICustomerRepository>();
        }

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType()
        {
            var handler = new RemoveCustomerCommandHandler(_loggerMoq.Object, _repositoryMoq.Object);

            Assert.NotNull(handler);
            Assert.IsType<RemoveCustomerCommandHandler>(handler);
        }

        [Fact]
        public async Task WhenEntityIsRemoved_ThenShouldReturnSuccess()
        {
            var handler = new RemoveCustomerCommandHandler(_loggerMoq.Object, _repositoryMoq.Object);

            var cmdResult = await handler.Handle(new RemoveCustomer(Guid.NewGuid()), CancellationToken.None);

            Assert.Equal(CommandOutcome.Accepted, cmdResult.Outcome);
        }

    }

}