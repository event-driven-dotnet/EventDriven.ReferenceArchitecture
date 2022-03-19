using System.Threading.Tasks;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Domain.CustomerAggregate.CommandHandlers;
using CustomerService.Repositories;
using EventDriven.CQRS.Abstractions.Commands;
using Moq;
using Xunit;

namespace CustomerService.Tests.Domain.CustomerAggregate.CommandHandlers;

public class CreateCustomerHandlerTests
{
    private readonly Mock<ICustomerRepository> _repositoryMock;

    public CreateCustomerHandlerTests()
    {
        _repositoryMock = new Mock<ICustomerRepository>();
    }

    [Fact]
    public void WhenInstantiated_ThenShouldBeOfCorrectType()
    {
        var handler = new CreateCustomerHandler(_repositoryMock.Object);

        Assert.NotNull(handler);
        Assert.IsType<CreateCustomerHandler>(handler);
    }

    [Fact]
    public async Task WhenCreatingEntityFails_ThenShouldReturnFailure()
    {
        var handler = new CreateCustomerHandler(_repositoryMock.Object);

        var cmdResult = await handler.Handle(new CreateCustomer(new Customer()), default);

        Assert.Equal(CommandOutcome.InvalidCommand, cmdResult.Outcome);
    }

    [Fact]
    public async Task WhenEntityIsCreated_ThenShouldReturnSuccess()
    {
        var customer = new Customer();
        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<Customer>()))
            .ReturnsAsync(customer);
        var handler = new CreateCustomerHandler(_repositoryMock.Object);

        var cmdResult = await handler.Handle(new CreateCustomer(customer), default);

        Assert.Equal(CommandOutcome.Accepted, cmdResult.Outcome);
    }

    [Fact]
    public async Task WhenEntityIsCreated_ThenShouldReturnNewEntity()
    {
        var customer = new Customer();
        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<Customer>()))
            .ReturnsAsync(customer);
        var handler = new CreateCustomerHandler(_repositoryMock.Object);

        var cmdResult = await handler.Handle(new CreateCustomer(customer), default);

        Assert.Equal(customer.Id,cmdResult.Entities[0].Id);
    }
}