using EventDriven.CQRS.Abstractions.Queries;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.OrderAggregate;
using OrderService.Domain.OrderAggregate.Queries;
using OrderService.DTO.Read;
using OrderState = OrderService.DTO.Read.OrderState;

namespace OrderService.Controllers;

[Route("api/order")]
[ApiController]
public class OrderQueryController : ControllerBase
{
    private readonly IQueryBroker _queryBroker;

    public OrderQueryController(
        IQueryBroker queryBroker)
    {
        _queryBroker = queryBroker;
    }

    // GET api/order
    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _queryBroker.SendAsync(new GetOrders());
        var result = MapOrderViews(orders);
        return Ok(result);
    }
    
    // GET api/order/customer/id
    [HttpGet]
    [Route("customer/{id:guid}")]
    public async Task<IActionResult> GetOrders([FromRoute] Guid id)
    {
        var orders = await _queryBroker.SendAsync(new GetOrdersByCustomer(id));
        var result = MapOrderViews(orders);
        return Ok(result);
    }

    // GET api/order/id
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetOrder([FromRoute] Guid id)
    {
        var order = await _queryBroker.SendAsync(new GetOrder(id));
        if (order == null) return NotFound();
        var result = MapOrderViews(Enumerable.Repeat(order, 1)).Single();
        return Ok(result);
    }
    
    private IEnumerable<OrderView> MapOrderViews(IEnumerable<Order?> orders) =>
        orders.Select(o => new OrderView
        {
            Id = o!.Id,
            CustomerId = o.CustomerId,
            OrderDate = o.OrderDate,
            OrderTotal = o.OrderItems.Sum(i => i.ProductPrice),
            Street = o.ShippingAddress.Street,
            City = o.ShippingAddress.City,
            State = o.ShippingAddress.State,
            Country = o.ShippingAddress.Country,
            PostalCode = o.ShippingAddress.PostalCode,
            OrderState = (OrderState)o.OrderState,
            ETag = o.ETag ?? string.Empty
        });
}