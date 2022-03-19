using AutoMapper;
using EventDriven.EventBus.Abstractions;
using Common.Integration.Events;
using OrderService.Domain.OrderAggregate;
using OrderService.Repositories;

namespace OrderService.Integration.EventHandlers;

public class CustomerAddressUpdatedEventHandler : IntegrationEventHandler<CustomerAddressUpdated>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerAddressUpdatedEventHandler> _logger;

    public CustomerAddressUpdatedEventHandler(IOrderRepository orderRepository,
        IMapper mapper,
        ILogger<CustomerAddressUpdatedEventHandler> logger)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public override async Task HandleAsync(CustomerAddressUpdated @event)
    {
        _logger.LogInformation("----- Handling CustomerAddressUpdated event");
        var orders = await _orderRepository.GetByCustomerAsync(@event.CustomerId);
        foreach (var order in orders)
        {
            var shippingAddress = _mapper.Map<Address>(@event.ShippingAddress);
            await _orderRepository.UpdateAddressAsync(order.Id, shippingAddress);
        }
    }
}