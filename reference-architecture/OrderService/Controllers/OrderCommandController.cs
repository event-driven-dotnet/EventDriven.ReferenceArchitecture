using AutoMapper;
using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.CQRS.Extensions;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.OrderAggregate;
using OrderService.Domain.OrderAggregate.Commands;

namespace OrderService.Controllers;

[Route("api/order")]
[ApiController]
public class OrderCommandController : ControllerBase
{
    private readonly ICommandBroker _commandBroker;
    private readonly IMapper _mapper;

    public OrderCommandController(
        ICommandBroker commandBroker,
        IMapper mapper)
    {
        _commandBroker = commandBroker;
        _mapper = mapper;
    }

    // POST api/order
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DTO.Write.Order orderDto)
    {
        var orderIn = _mapper.Map<Order>(orderDto);
        var result = await _commandBroker.SendAsync(new CreateOrder(orderIn));

        if (result.Outcome != CommandOutcome.Accepted)
            return result.ToActionResult();
        var orderOut = _mapper.Map<DTO.Write.Order>(result.Entity);
        return new CreatedResult($"api/order/{orderOut.Id}", orderOut);
    }

    // PUT api/order
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] DTO.Write.Order orderDto)
    {
        var orderIn = _mapper.Map<Order>(orderDto);
        var result = await _commandBroker.SendAsync(new UpdateOrder(orderIn));

        if (result.Outcome != CommandOutcome.Accepted)
            return result.ToActionResult();
        var orderOut = _mapper.Map<DTO.Write.Order>(result.Entity);
        return result.ToActionResult(orderOut);
    }

    // DELETE api/order
    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> Remove([FromRoute] Guid id)
    {
        var result = await _commandBroker.SendAsync(new RemoveOrder(id));
        return result.Outcome != CommandOutcome.Accepted
            ? result.ToActionResult() 
            : new NoContentResult();
    }

    // PUT api/order/ship
    [HttpPut]
    [Route("ship/{id:guid}/{etag}")]
    public async Task<IActionResult> Ship([FromRoute] Guid id)
    {
        var result = await _commandBroker.SendAsync(new ShipOrder(id));

        if (result.Outcome != CommandOutcome.Accepted)
            return result.ToActionResult();
        var orderOut = _mapper.Map<DTO.Write.Order>(result.Entity);
        return result.ToActionResult(orderOut);
    }

    // PUT api/order/cancel
    [HttpPut]
    [Route("cancel/{id:guid}/{etag}")]
    public async Task<IActionResult> Cancel([FromRoute] Guid id)
    {
        var result = await _commandBroker.SendAsync(new CancelOrder(id));

        if (result.Outcome != CommandOutcome.Accepted)
            return result.ToActionResult();
        var orderOut = _mapper.Map<DTO.Write.Order>(result.Entity);
        return result.ToActionResult(orderOut);
    }
}