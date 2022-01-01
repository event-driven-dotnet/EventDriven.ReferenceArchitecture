using System;
using System.Threading.Tasks;
using AutoMapper;
using EventDriven.DDD.Abstractions.Commands;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.OrderAggregate;
using OrderService.Domain.OrderAggregate.CommandHandlers;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Helpers;

namespace OrderService.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderCommandController : ControllerBase
    {
        private readonly OrderCommandHandler _commandHandler;
        private readonly IMapper _mapper;

        public OrderCommandController(OrderCommandHandler commandHandler, IMapper mapper)
        {
            _commandHandler = commandHandler;
            _mapper = mapper;
        }

        // POST api/order
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DTO.Write.Order orderDto)
        {
            var orderIn = _mapper.Map<Order>(orderDto);
            var result = await _commandHandler.Handle(new CreateOrder(orderIn));

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
            var result = await _commandHandler.Handle(new UpdateOrder(orderIn));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var orderOut = _mapper.Map<DTO.Write.Order>(result.Entity);
            return result.ToActionResult(orderOut);
        }

        // DELETE api/order
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Remove([FromRoute] Guid id)
        {
            var result = await _commandHandler.Handle(new RemoveOrder(id));
            return result.Outcome != CommandOutcome.Accepted
                ? result.ToActionResult() 
                : new NoContentResult();
        }

        // PUT api/order/ship
        [HttpPut]
        [Route("ship/{id}/{etag}")]
        public async Task<IActionResult> Ship([FromRoute] Guid id, string etag)
        {
            var result = await _commandHandler.Handle(new ShipOrder(id, etag));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var orderOut = _mapper.Map<DTO.Write.Order>(result.Entity);
            return result.ToActionResult(orderOut);
        }

        // PUT api/order/cancel
        [HttpPut]
        [Route("cancel/{id}/{etag}")]
        public async Task<IActionResult> Cancel([FromRoute] Guid id, string etag)
        {
            var result = await _commandHandler.Handle(new CancelOrder(id, etag));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var orderOut = _mapper.Map<DTO.Write.Order>(result.Entity);
            return result.ToActionResult(orderOut);
        }
    }
}
