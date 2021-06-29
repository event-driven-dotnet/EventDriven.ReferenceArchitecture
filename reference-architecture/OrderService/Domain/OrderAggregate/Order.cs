using System;
using System.Collections.Generic;
using EventDriven.CQRS.Abstractions.Entities;
using EventDriven.CQRS.Abstractions.Events;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Domain.OrderAggregate.Events;

namespace OrderService.Domain.OrderAggregate
{
    public class Order : Entity,
                         IOrderAggregate
    {
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public Address ShippingAddress { get; set; }
        public OrderState OrderState { get; set; }

        public IEnumerable<IDomainEvent> Process(CreateOrder command)

            // To process command, return one or more domain events
            => new List<IDomainEvent>
            {
                new OrderCreated(command.Order)
            };

        public void Apply(OrderCreated domainEvent) =>

            // Set Id
            Id = domainEvent.EntityId != default ? domainEvent.EntityId : Guid.NewGuid();

        public IEnumerable<IDomainEvent> Process(ShipOrder command)

            // To process command, return one or more domain events
            => new List<IDomainEvent>
            {
                new OrderShipped(command.EntityId, command.ETag)
            };

        public void Apply(OrderShipped domainEvent)
        {
            // To apply events, mutate the entity state
            OrderState = OrderState.Shipped;
            ETag = domainEvent.ETag;
        }

        public IEnumerable<IDomainEvent> Process(CancelOrder command)

            // To process command, return one or more domain events
            => new List<IDomainEvent>
            {
                new OrderCancelled(command.EntityId, command.ETag)
            };

        public void Apply(OrderCancelled domainEvent)
        {
            // To apply events, mutate the entity state
            OrderState = OrderState.Cancelled;
            ETag = domainEvent.ETag;
        }
    }
}