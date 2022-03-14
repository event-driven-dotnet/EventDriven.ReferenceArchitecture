using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Entities;
using EventDriven.DDD.Abstractions.Events;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Domain.OrderAggregate.Events;

namespace OrderService.Domain.OrderAggregate;

public class Order : 
    Entity,
    ICommandProcessor<CreateOrder, Order, OrderCreated>,
    IEventApplier<OrderCreated>,
    ICommandProcessor<UpdateOrder, Order, OrderUpdated>,
    IEventApplier<OrderUpdated>,
    ICommandProcessor<RemoveOrder, OrderRemoved>,
    IEventApplier<OrderRemoved>,
    ICommandProcessor<ShipOrder, Order, OrderShipped>,
    IEventApplier<OrderShipped>,
    ICommandProcessor<CancelOrder, Order, OrderCancelled>,
    IEventApplier<OrderCancelled>
{
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItem> OrderItems { get; set; } = null!;
    public Address ShippingAddress { get; set; } = null!;
    public OrderState OrderState { get; set; }

    public OrderCreated Process(CreateOrder command)
        // To process command, return one or more domain events
        => new(command.Entity);

    public void Apply(OrderCreated domainEvent) =>
        // Set Id
        Id = domainEvent.EntityId != default ? domainEvent.EntityId : Guid.NewGuid();

    public OrderUpdated Process(UpdateOrder command) =>
        // To process command, return a domain event
        new(command.Entity);

    public void Apply(OrderUpdated domainEvent)
    {
        // Set ETag
        if (domainEvent.EntityETag != null) ETag = domainEvent.EntityETag;
    }

    public OrderRemoved Process(RemoveOrder command) =>
        // To process command, return a domain event
        new(command.EntityId);

    public void Apply(OrderRemoved domainEvent)
    {
        // Could mutate state here to implement a soft delete
    }
    public OrderShipped Process(ShipOrder command) =>
        // To process command, return one or more domain events
        new(command.EntityId);

    public void Apply(OrderShipped domainEvent)
    {
        // To apply events, mutate the entity state
        OrderState = OrderState.Shipped;
        if (domainEvent.EntityETag != null) ETag = domainEvent.EntityETag;
    }

    public OrderCancelled Process(CancelOrder command) =>
        // To process command, return one or more domain events
        new(command.EntityId);

    public void Apply(OrderCancelled domainEvent)
    {
        // To apply events, mutate the entity state
        OrderState = OrderState.Cancelled;
        if (domainEvent.EntityETag != null) ETag = domainEvent.EntityETag;
    }
}