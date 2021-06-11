namespace CustomerService.Controllers
{

    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using Domain.CustomerAggregate.Commands;
    using DTO.Write;
    using EventDriven.CQRS.Abstractions.Commands;
    using Helpers;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/customer")]
    [ApiController]
    public class CustomerCommandController : ControllerBase
    {
        private readonly ICommandBroker commandBroker;
        private readonly IMapper mapper;

        public CustomerCommandController(ICommandBroker commandBroker, IMapper mapper)
        {
            this.commandBroker = commandBroker;
            this.mapper = mapper;
        }

        // POST api/customer
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Customer customerDto)
        {
            var customerIn = mapper.Map<Domain.CustomerAggregate.Customer>(customerDto);
            var result = await commandBroker.InvokeAsync<CreateCustomer, CommandResult<Domain.CustomerAggregate.Customer>>(new CreateCustomer(customerIn));
            
            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var customerOut = mapper.Map<Customer>(result.Entity);
            return new CreatedResult($"api/customer/{customerOut.Id}", customerOut);
        }

        // PUT api/customer
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Customer customerDto)
        {
            var customerIn = mapper.Map<Domain.CustomerAggregate.Customer>(customerDto);
            var result = await commandBroker.InvokeAsync<UpdateCustomer, CommandResult<Domain.CustomerAggregate.Customer>>(new UpdateCustomer(customerIn));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var customerOut = mapper.Map<Customer>(result.Entity);
            return result.ToActionResult(customerOut);
        }

        // DELETE api/customer/id
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Remove([FromRoute] Guid id) {
            var result = await commandBroker.InvokeAsync<RemoveCustomer, CommandResult>(new RemoveCustomer(id));
            return result.Outcome != CommandOutcome.Accepted
                ? result.ToActionResult() 
                : new NoContentResult();
        }
    }
}
