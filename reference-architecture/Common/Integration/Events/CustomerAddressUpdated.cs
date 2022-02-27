using Common.Integration.Models;
using EventDriven.EventBus.Abstractions;

namespace Common.Integration.Events
{
    public record CustomerAddressUpdated(Guid CustomerId, Address ShippingAddress) : IntegrationEvent;
}