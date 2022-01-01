using System;
using System.Threading.Tasks;
using AutoMapper;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Domain.CustomerAggregate.CommandHandlers;
using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Helpers;
using EventDriven.DDD.Abstractions.Commands;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerCommandController : ControllerBase
    {
        private readonly CustomerCommandHandler _commandHandler;
        private readonly IMapper _mapper;

        public CustomerCommandController(CustomerCommandHandler commandHandler, IMapper mapper)
        {
            _commandHandler = commandHandler;
            _mapper = mapper;
        }

        // POST api/customer
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DTO.Write.Customer customerDto)
        {
            var customerIn = _mapper.Map<Customer>(customerDto);
            var result = await _commandHandler.Handle(new CreateCustomer(customerIn));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var customerOut = _mapper.Map<DTO.Write.Customer>(result.Entity);
            return new CreatedResult($"api/customer/{customerOut.Id}", customerOut);
        }

        // PUT api/customer
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] DTO.Write.Customer customerDto)
        {
            var customerIn = _mapper.Map<Customer>(customerDto);
            var result = await _commandHandler.Handle(new UpdateCustomer(customerIn));

            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var customerOut = _mapper.Map<DTO.Write.Customer>(result.Entity);
            return result.ToActionResult(customerOut);
        }

        // DELETE api/customer/id
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Remove([FromRoute] Guid id)
        {
            var result = await _commandHandler.Handle(new RemoveCustomer(id));
            return result.Outcome != CommandOutcome.Accepted
                ? result.ToActionResult() 
                : new NoContentResult();
        }
    }
}
