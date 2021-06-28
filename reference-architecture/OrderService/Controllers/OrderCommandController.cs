using System;
using System.Threading.Tasks;
using AutoMapper;
using EventDriven.CQRS.Abstractions.Commands;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.DTO.Write;
using OrderService.Helpers;

namespace OrderService.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderCommandController : ControllerBase
    {
        private readonly ICommandBroker _commandBroker;
        private readonly IMapper _mapper;

        public OrderCommandController(ICommandBroker commandBroker, IMapper mapper)
        {
            _commandBroker = commandBroker;
            _mapper = mapper;
        }

        // POST api/order
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Order orderDto)
        {
            var orderIn = _mapper.Map<Domain.OrderAggregate.Order>(orderDto);
            var result = await _commandBroker.InvokeAsync<CreateOrder, CommandResult<Domain.OrderAggregate.Order>>(new CreateOrder(orderIn));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();

            var orderOut = _mapper.Map<Order>(result.Entity);
            return new CreatedResult($"api/order/{orderOut.Id}", orderOut);
        }

        // PUT api/order
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Order orderDto)
        {
            var orderIn = _mapper.Map<Domain.OrderAggregate.Order>(orderDto);
            var result = await _commandBroker.InvokeAsync<UpdateOrder, CommandResult<Domain.OrderAggregate.Order>>(new UpdateOrder(orderIn));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();

            var orderOut = _mapper.Map<Order>(result.Entity);
            return result.ToActionResult(orderOut);
        }

        // DELETE api/order
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Remove([FromRoute] Guid id)
        {
            var result = await _commandBroker.InvokeAsync<RemoveOrder, CommandResult>(new RemoveOrder(id));
            return result.Outcome != CommandOutcome.Accepted ? result.ToActionResult() : new NoContentResult();
        }

        // PUT api/order/ship
        [HttpPut]
        [Route("ship/{id}/{etag}")]
        public async Task<IActionResult> Ship([FromRoute] Guid id, string etag)
        {
            var result = await _commandBroker.InvokeAsync<ShipOrder, CommandResult<Domain.OrderAggregate.Order>>(new ShipOrder(id, etag));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();

            var orderOut = _mapper.Map<Order>(result.Entity);
            return result.ToActionResult(orderOut);
        }

        // PUT api/order/cancel
        [HttpPut]
        [Route("cancel/{id}/{etag}")]
        public async Task<IActionResult> Cancel([FromRoute] Guid id, string etag)
        {
            var result = await _commandBroker.InvokeAsync<CancelOrder, CommandResult<Domain.OrderAggregate.Order>>(new CancelOrder(id, etag));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();

            var orderOut = _mapper.Map<Order>(result.Entity);
            return result.ToActionResult(orderOut);
        }
    }
}