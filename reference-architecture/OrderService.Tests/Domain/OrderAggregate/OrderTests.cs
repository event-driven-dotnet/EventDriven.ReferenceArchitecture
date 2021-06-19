using System;
using System.Collections.Generic;
using AutoMapper;
using EventDriven.CQRS.Abstractions.Events;
using OrderService.Domain.OrderAggregate;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Domain.OrderAggregate.Events;
using OrderService.Tests.Fakes;
using OrderService.Utils;
using Xunit;

namespace OrderService.Tests.Domain.OrderAggregate
{

    public class OrderTests
    {

        private readonly IMapper _mapper;

        public OrderTests() => _mapper = BaseUtils.GetMapper();

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType()
        {
            var order = new Order();

            Assert.NotNull(order);
            Assert.IsType<Order>(order);
        }

        [Fact]
        public void WhenProcessingCreateOrderCommand_ThenShouldReturnCollectionOfDomainEvents()
        {
            var orderIn = _mapper.Map<Order>(Orders.Order1);
            var order = new Order();

            var events = order.Process(new CreateOrder(orderIn));

            Assert.NotNull(events);
            Assert.NotEmpty(events);
            Assert.IsAssignableFrom<IEnumerable<IDomainEvent>>(events);
        }

        [Fact]
        public void WhenProcessingShipOrderCommand_ThenShouldReturnCollectionOfDomainEvents()
        {
            var orderIn = _mapper.Map<Order>(Orders.Order1);
            var order = new Order();

            var events = order.Process(new ShipOrder(Guid.Empty, string.Empty));

            Assert.NotNull(events);
            Assert.NotEmpty(events);
            Assert.IsAssignableFrom<IEnumerable<IDomainEvent>>(events);
        }

        [Fact]
        public void WhenProcessingCancelOrderCommand_ThenShouldReturnCollectionOfDomainEvents()
        {
            var orderIn = _mapper.Map<Order>(Orders.Order1);
            var order = new Order();

            var events = order.Process(new CancelOrder(Guid.Empty, string.Empty));

            Assert.NotNull(events);
            Assert.NotEmpty(events);
            Assert.IsAssignableFrom<IEnumerable<IDomainEvent>>(events);
        }

        [Fact]
        public void WhenApplyingOrderCreatedEvent_ThenShouldHaveNewId()
        {
            var order = new Order();
            var createdEvent = new OrderCreated(order)
            {
                EntityId = Guid.NewGuid()
            };

            order.Apply(createdEvent);

            Assert.Equal(createdEvent.EntityId, order.Id);
        }

        [Fact]
        public void WhenApplyingOrderShippedEvent_ThenShouldBeInShippedState()
        {
            var order = _mapper.Map<Order>(Orders.Order1);
            var shippedEvent = new OrderShipped(order.Id, order.ETag);

            order.Apply(shippedEvent);

            Assert.Equal(OrderState.Shipped, order.OrderState);
        }

        [Fact]
        public void WhenApplyingOrderCancelledEvent_ThenShouldBeIncancelledState()
        {
            var order = _mapper.Map<Order>(Orders.Order1);
            var cancelledEvent = new OrderCancelled(order.Id, order.ETag);

            order.Apply(cancelledEvent);

            Assert.Equal(OrderState.Cancelled, order.OrderState);
        }

    }

}