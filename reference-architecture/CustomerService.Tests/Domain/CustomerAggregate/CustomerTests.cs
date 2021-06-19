using System;
using System.Collections.Generic;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Domain.CustomerAggregate.Events;
using EventDriven.CQRS.Abstractions.Events;
using Xunit;

namespace CustomerService.Tests.Domain.CustomerAggregate
{

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
        public void WhenProcessingCreateCustomerCommand_ThenShouldReturnCollectionOfDomainEvents()
        {
            var customer = new Customer();

            var events = customer.Process(new CreateCustomer(new Customer
            {
                Id = Guid.NewGuid()
            }));

            Assert.NotNull(events);
            Assert.NotEmpty(events);
            Assert.IsAssignableFrom<IEnumerable<IDomainEvent>>(events);
        }

        [Fact]
        public void WhenApplyingCustomerCreatedEvent_ThenShouldHaveIdSaved()
        {
            var customer = new Customer();
            var customerCreated = new CustomerCreated(customer)
            {
                EntityId = Guid.NewGuid()
            };

            customer.Apply(customerCreated);

            Assert.Equal(customerCreated.EntityId, customer.Id);
        }

    }

}