using AutoMapper;
using CustomerService.Domain.CustomerAggregate.Queries;
using CustomerService.DTO.Read;
using EventDriven.CQRS.Abstractions.Queries;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers;

[Route("api/customer")]
[ApiController]
public class CustomerQueryController : ControllerBase
{
    private readonly IQueryBroker _queryBroker;
    private readonly IMapper _mapper;

    public CustomerQueryController(
        IQueryBroker queryBroker,
        IMapper mapper)
    {
        _queryBroker = queryBroker;
        _mapper = mapper;
    }

    // GET api/customer
    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        var customers = await _queryBroker.SendAsync(new GetCustomers());
        var result = _mapper.Map<IEnumerable<CustomerView>>(customers);
        return Ok(result);
    }

    // GET api/customer/id
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetCustomer([FromRoute] Guid id)
    {
        var customer = await _queryBroker.SendAsync(new GetCustomer(id));
        if (customer == null) return NotFound();
        var result = _mapper.Map<CustomerView>(customer);
        return Ok(result);
    }
}