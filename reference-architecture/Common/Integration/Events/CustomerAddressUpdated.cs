using System;
using System.Diagnostics.CodeAnalysis;
using Common.Integration.Models;
using EventDriven.EventBus.Abstractions;

namespace Common.Integration.Events
{
    [ExcludeFromCodeCoverage]
    public record CustomerAddressUpdated(Guid CustomerId, Address ShippingAddress) : IntegrationEvent;
}