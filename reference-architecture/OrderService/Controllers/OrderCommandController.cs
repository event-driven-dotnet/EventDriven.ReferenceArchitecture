using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.OrderAggregate;

namespace OrderService.Controllers
{

    using Domain.OrderAggregate.Commands;
    using EventDriven.CQRS.Abstractions.Commands;
    using Helpers;

    [Route("api/order")]
    [ApiController]
    public class OrderCommandController : ControllerBase
    {

        private readonly ICommandBroker commandBroker;
        private readonly IMapper mapper;

        public OrderCommandController(ICommandBroker commandBroker, IMapper mapper)
        {
            this.commandBroker = commandBroker;
            this.mapper = mapper;
        }

        // POST api/order
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DTO.Write.Order orderDto)
        {
            var orderIn = mapper.Map<Order>(orderDto);
            var result = await commandBroker.InvokeAsync<CreateOrder, CommandResult<Order>>(new CreateOrder(orderIn));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var orderOut = mapper.Map<DTO.Write.Order>(result.Entity);
            return new CreatedResult($"api/order/{orderOut.Id}", orderOut);
        }

        // PUT api/order
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] DTO.Write.Order orderDto)
        {
            var orderIn = mapper.Map<Order>(orderDto);
            var result = await commandBroker.InvokeAsync<UpdateOrder, CommandResult<Order>>(new UpdateOrder(orderIn));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var orderOut = mapper.Map<DTO.Write.Order>(result.Entity);
            return result.ToActionResult(orderOut);
        }

        // DELETE api/order
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Remove([FromRoute] Guid id) {
            var result = await commandBroker.InvokeAsync<RemoveOrder, CommandResult>(new RemoveOrder(id));
            return result.Outcome != CommandOutcome.Accepted
                ? result.ToActionResult() 
                : new NoContentResult();
        }

        // PUT api/order/ship
        [HttpPut]
        [Route("ship/{id}/{etag}")]
        public async Task<IActionResult> Ship([FromRoute] Guid id, string etag) {
            var result = await commandBroker.InvokeAsync<ShipOrder, CommandResult<Order>>(new ShipOrder(id, etag));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var orderOut = mapper.Map<DTO.Write.Order>(result.Entity);
            return result.ToActionResult(orderOut);
        }

        // PUT api/order/cancel
        [HttpPut]
        [Route("cancel/{id}/{etag}")]
        public async Task<IActionResult> Cancel([FromRoute] Guid id, string etag) {
            var result = await commandBroker.InvokeAsync<CancelOrder, CommandResult<Order>>(new CancelOrder(id, etag));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var orderOut = mapper.Map<DTO.Write.Order>(result.Entity);
            return result.ToActionResult(orderOut);
        }
    }
}
