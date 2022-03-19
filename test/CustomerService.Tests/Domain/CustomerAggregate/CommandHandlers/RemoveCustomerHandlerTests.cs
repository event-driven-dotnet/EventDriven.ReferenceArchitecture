using System;
using System.Threading.Tasks;
using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Domain.CustomerAggregate.CommandHandlers;
using CustomerService.Repositories;
using EventDriven.CQRS.Abstractions.Commands;
using Moq;
using Xunit;

namespace CustomerService.Tests.Domain.CustomerAggregate.CommandHandlers;

public class RemoveCustomerHandlerTests
{

    private readonly Mock<ICustomerRepository> _repositoryMock;

    public RemoveCustomerHandlerTests()
    {
        _repositoryMock = new Mock<ICustomerRepository>();
    }

    [Fact]
    public void WhenInstantiated_ThenShouldBeOfCorrectType()
    {
        var handler = new RemoveCustomerHandler(_repositoryMock.Object);

        Assert.NotNull(handler);
        Assert.IsType<RemoveCustomerHandler>(handler);
    }

    [Fact]
    public async Task WhenEntityIsRemoved_ThenShouldReturnSuccess()
    {
        var handler = new RemoveCustomerHandler(_repositoryMock.Object);

        var cmdResult = await handler.Handle(new RemoveCustomer(Guid.NewGuid()), default);

        Assert.Equal(CommandOutcome.Accepted, cmdResult.Outcome);
    }
}