using System;
using System.Collections.Generic;
using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Entities;
using EventDriven.DDD.Abstractions.Events;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Domain.OrderAggregate.Events;

namespace OrderService.Domain.OrderAggregate
{
    public class Order : 
        Entity,
        ICommandProcessor<CreateOrder, OrderCreated>,
        IEventApplier<OrderCreated>,
        ICommandProcessor<ShipOrder, OrderShipped>,
        IEventApplier<OrderShipped>,
        ICommandProcessor<CancelOrder, OrderCancelled>,
        IEventApplier<OrderCancelled>
    {
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public Address ShippingAddress { get; set; }
        public OrderState OrderState { get; set; }

        public OrderCreated Process(CreateOrder command)
            // To process command, return one or more domain events
            => new(command.Entity);

        public void Apply(OrderCreated domainEvent) =>
            // Set Id
            Id = domainEvent.EntityId != default(Guid) ? domainEvent.EntityId : Guid.NewGuid();

        public OrderShipped Process(ShipOrder command)
            // To process command, return one or more domain events
            => new OrderShipped(command.EntityId, command.ETag);

        public void Apply(OrderShipped domainEvent)
        {
            // To apply events, mutate the entity state
            OrderState = OrderState.Shipped;
            ETag = domainEvent.ETag;
        }

        public OrderCancelled Process(CancelOrder command)
            // To process command, return one or more domain events
            => new(command.EntityId, command.ETag);

        public void Apply(OrderCancelled domainEvent)
        {
            // To apply events, mutate the entity state
            OrderState = OrderState.Cancelled;
            ETag = domainEvent.ETag;
        }
    }
}