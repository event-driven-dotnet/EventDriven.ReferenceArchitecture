using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.OrderAggregate;
using OrderService.DTO.Read;
using OrderService.Repositories;
using OrderState = OrderService.DTO.Read.OrderState;

namespace OrderService.Controllers;

[Route("api/order")]
[ApiController]
public class OrderQueryController : ControllerBase
{
    private readonly IOrderRepository _repository;

    public OrderQueryController(IOrderRepository repository)
    {
        _repository = repository;
    }

    // GET api/order
    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _repository.GetAsync();
        var result = GetOrderViews(orders);
        return Ok(result);
    }
        
    // GET api/order/customer/id
    [HttpGet]
    [Route("customer/{id}")]
    public async Task<IActionResult> GetOrders([FromRoute] Guid id)
    {
        var orders = await _repository.GetByCustomerAsync(id);
        var result = GetOrderViews(orders);
        return Ok(result);
    }

    // GET api/order/id
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetOrder([FromRoute] Guid id)
    {
        var order = await _repository.GetAsync(id);
        var result = GetOrderViews
            (Enumerable.Repeat(order, 1)).Single();
        return Ok(result);
    }

    private IEnumerable<OrderView> GetOrderViews(IEnumerable<Order?> orders) =>
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
            OrderState = (OrderState) o.OrderState,
            ETag = o.ETag
        });
}