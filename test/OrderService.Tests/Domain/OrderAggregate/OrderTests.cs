using System;
using AutoMapper;
using OrderService.Domain.OrderAggregate;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Domain.OrderAggregate.Events;
using OrderService.Tests.Fakes;
using OrderService.Tests.Helpers;
using Xunit;

namespace OrderService.Tests.Domain.OrderAggregate;

public class OrderTests
{
    private readonly IMapper _mapper;

    public OrderTests() => _mapper = MappingHelper.GetMapper();

    [Fact]
    public void WhenInstantiated_ThenShouldBeOfCorrectType()
    {
        var order = new Order();

        Assert.NotNull(order);
        Assert.IsType<Order>(order);
    }

    [Fact]
    public void WhenProcessingCreateOrderCommand_ThenShouldReturnOrderCreated()
    {
        var orderIn = _mapper.Map<Order>(Orders.Order1);
        var order = new Order();

        var @event = order.Process(new CreateOrder(orderIn));

        Assert.NotNull(@event);
        Assert.IsAssignableFrom<OrderCreated>(@event);
    }

    [Fact]
    public void WhenApplyingOrderCreatedEvent_ThenShouldHaveIdSet()
    {
        var order = new Order
        {
            Id = Guid.NewGuid()
        };
        var createdEvent = new OrderCreated(order);

        order.Apply(createdEvent);

        Assert.Equal(createdEvent.EntityId, order.Id);
    }

    [Fact]
    public void WhenProcessingUpdateOrderCommand_ThenShouldReturnOrderUpdated()
    {
        var orderIn = _mapper.Map<Order>(Orders.Order1);
        var order = new Order();

        var @event = order.Process(new UpdateOrder(orderIn));

        Assert.NotNull(@event);
        Assert.IsAssignableFrom<OrderUpdated>(@event);
    }

    [Fact]
    public void WhenApplyingOrderUpdatedEvent_ThenShouldHaveETagSet()
    {
        var order = new Order
        {
            Id = Guid.NewGuid()
        };
        var createdEvent = new OrderUpdated(order);

        order.Apply(createdEvent);

        Assert.Equal(createdEvent.EntityETag, order.ETag);
    }

    [Fact]
    public void WhenProcessingRemoveOrderCommand_ThenShouldReturnOrderRemoved()
    {
        var order = new Order();

        var @event = order.Process(new RemoveOrder(Guid.NewGuid()));

        Assert.NotNull(@event);
        Assert.IsAssignableFrom<OrderRemoved>(@event);
    }

    [Fact]
    public void WhenApplyingOrderRemovedEvent_ThenShouldBeNotNull()
    {
        var order = new Order();
        var orderRemoved = new OrderRemoved(Guid.NewGuid());

        order.Apply(orderRemoved);

        Assert.NotNull(order);
    }

    [Fact]
    public void WhenProcessingShipOrderCommand_ThenShouldReturnOrderShipped()
    {
        var orderIn = _mapper.Map<Order>(Orders.Order1);
        var order = new Order();

        var events = order.Process(new ShipOrder(orderIn.Id));

        Assert.NotNull(events);
        Assert.IsAssignableFrom<OrderShipped>(events);
    }

    [Fact]
    public void WhenApplyingOrderShippedEvent_ThenShouldBeInShippedState()
    {
        var orderIn = _mapper.Map<Order>(Orders.Order1);
        var shippedEvent = new OrderShipped(orderIn.Id);

        orderIn.Apply(shippedEvent);

        Assert.Equal(OrderState.Shipped, orderIn.OrderState);
    }

    [Fact]
    public void WhenProcessingCancelOrderCommand_ThenShouldReturnOrderCancelled()
    {
        var orderIn = _mapper.Map<Order>(Orders.Order1);
        var order = new Order();

        var events = order.Process(new CancelOrder(orderIn.Id));

        Assert.NotNull(events);
        Assert.IsAssignableFrom<OrderCancelled>(events);
    }

    [Fact]
    public void WhenApplyingOrderCancelledEvent_ThenShouldBeInCancelledState()
    {
        var orderIn = _mapper.Map<Order>(Orders.Order1);
        var cancelledEvent = new OrderCancelled(orderIn.Id);

        orderIn.Apply(cancelledEvent);

        Assert.Equal(OrderState.Cancelled, orderIn.OrderState);
    }
}