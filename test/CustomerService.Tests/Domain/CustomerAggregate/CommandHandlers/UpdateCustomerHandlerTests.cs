using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Common.Integration.Events;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Domain.CustomerAggregate.CommandHandlers;
using CustomerService.Repositories;
using CustomerService.Tests.Helpers;
using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CustomerService.Tests.Domain.CustomerAggregate.CommandHandlers;

public class UpdateCustomerHandlerTests
{
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly Fixture _fixture = new();

    private readonly Mock<ILogger<UpdateCustomerHandler>> _loggerMock;
    private readonly IMapper _mapper = MappingHelper.GetMapper();
    private readonly Mock<ICustomerRepository> _repositoryMock;

    public UpdateCustomerHandlerTests()
    {
        _loggerMock = new Mock<ILogger<UpdateCustomerHandler>>();
        _repositoryMock = new Mock<ICustomerRepository>();
        _eventBusMock = new Mock<IEventBus>();
    }

    [Fact]
    public void WhenInstantiated_ThenShouldBeOfCorrectType()
    {
        var handler = GetHandler();

        Assert.NotNull(handler);
        Assert.IsType<UpdateCustomerHandler>(handler);
    }

    [Fact]
    public async Task WhenNoExistingCustomerIsFound_ThenShouldReturnNotFound()
    {
        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(GenerateCustomer());

        var handler = GetHandler();

        var cmdResult = await handler.Handle(new UpdateCustomer(GenerateCustomer()), default);

        Assert.Equal(CommandOutcome.NotFound, cmdResult.Outcome);
    }

    [Fact]
    public async Task WhenTheAddressIsUpdated_ThenEventShouldBePublished()
    {
        var existingCustomer = GenerateCustomer();
        var updatedCustomer = GenerateCustomer();
        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingCustomer);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Customer>()))
            .ReturnsAsync(updatedCustomer);
        var eventRaised = false;
        _eventBusMock.Setup(x => x.PublishAsync(
        It.IsAny<CustomerAddressUpdated>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Callback(() => eventRaised = true);
        var handler = GetHandler();

        await handler.Handle(new UpdateCustomer(updatedCustomer), default);

        Assert.True(eventRaised);
    }

    [Fact]
    public async Task WhenTheAddressIsNotUpdated_ThenEventShouldNotBePublished()
    {
        var existingCustomer = GenerateCustomer();
        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingCustomer);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Customer>()))
            .ReturnsAsync(existingCustomer);
        var eventRaised = false;
        _eventBusMock.Setup(x => x.PublishAsync(
        It.IsAny<CustomerAddressUpdated>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Callback(() => eventRaised = true);
        var handler = GetHandler();

        await handler.Handle(new UpdateCustomer(existingCustomer), default);

        Assert.False(eventRaised);
    }

    [Fact]
    public async Task WhenTheCustomerIsUpdated_ThenShouldReturnSuccess()
    {
        var existingCustomer = GenerateCustomer();
        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingCustomer);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Customer>()))
            .ReturnsAsync(existingCustomer);
        var handler = GetHandler();

        var cmdResult = await handler.Handle(new UpdateCustomer(existingCustomer), default);

        Assert.Equal(CommandOutcome.Accepted, cmdResult.Outcome);
    }

    [Fact]
    public async Task WhenConcurrencyExceptionOccurs_ThenShouldReturnConflict()
    {
        var existingCustomer = GenerateCustomer();
        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingCustomer);
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Customer>()))
            .ThrowsAsync(new ConcurrencyException());
        var handler = GetHandler();

        var cmdResult = await handler.Handle(new UpdateCustomer(existingCustomer), default);

        Assert.Equal(CommandOutcome.Conflict, cmdResult.Outcome);
    }

    private UpdateCustomerHandler GetHandler() =>
        new(_repositoryMock.Object,
            _eventBusMock.Object,
            _mapper,
            _loggerMock.Object);

    private Customer GenerateCustomer() =>
        _fixture.Build<Customer>()
            .With(x => x.ShippingAddress)
            .With(x => x.Id)
            .Create();
}