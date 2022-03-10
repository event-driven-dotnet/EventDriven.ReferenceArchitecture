using System;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Domain.CustomerAggregate.Events;
using Xunit;

namespace CustomerService.Tests.Domain.CustomerAggregate;

public class CustomerTests
{
    [Fact]
    public void WhenInstantiated_ThenShouldBeOfCorrectType()
    {
        var customer = new Customer();

        Assert.NotNull(customer);
        Assert.IsType<Customer>(customer);
    }

    [Fact]
    public void WhenProcessingCreateCustomerCommand_ThenShouldReturnCustomerCreated()
    {
        var customer = new Customer();

        var @event = customer.Process(new CreateCustomer(new Customer
        {
            Id = Guid.NewGuid()
        }));

        Assert.NotNull(@event);
        Assert.IsAssignableFrom<CustomerCreated>(@event);
    }
    
    [Fact]
    public void WhenApplyingCustomerCreatedEvent_ThenShouldHaveIdSet()
    {

        var customer = new Customer
        {
            Id = Guid.NewGuid()
        };
        var customerCreated = new CustomerCreated(customer);

        customer.Apply(customerCreated);

        Assert.Equal(customerCreated.EntityId, customer.Id);
    }
    
    [Fact]
    public void WhenProcessingUpdateCustomerCommand_ThenShouldReturnCustomerUpdated()
    {
        var customer = new Customer();

        var @event = customer.Process(new UpdateCustomer(new Customer
        {
            Id = Guid.NewGuid()
        }));

        Assert.NotNull(@event);
        Assert.IsAssignableFrom<CustomerUpdated>(@event);
    }
    
    [Fact]
    public void WhenApplyingCustomerUpdatedEvent_ThenShouldHaveETagSet()
    {
        var customer = new Customer();
        var customerUpdated = new CustomerUpdated(customer);

        customer.Apply(customerUpdated);

        Assert.Equal(customerUpdated.EntityETag, customer.ETag);
    }
    
    [Fact]
    public void WhenProcessingRemoveCustomerCommand_ThenShouldReturnCustomerRemoved()
    {
        var customer = new Customer();

        var @event = customer.Process(new RemoveCustomer(Guid.NewGuid()));

        Assert.NotNull(@event);
        Assert.IsAssignableFrom<CustomerRemoved>(@event);
    }
    
    [Fact]
    public void WhenApplyingCustomerRemovedEvent_ThenShouldBeNotNull()
    {
        var customer = new Customer();
        var customerRemoved = new CustomerRemoved(Guid.NewGuid());

        customer.Apply(customerRemoved);

        Assert.NotNull(customer);
    }
}