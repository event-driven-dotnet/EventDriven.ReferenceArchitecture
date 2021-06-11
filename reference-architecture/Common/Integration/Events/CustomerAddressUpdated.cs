namespace Common.Integration.Events {

    using System;
    using System.Diagnostics.CodeAnalysis;
    using EventDriven.EventBus.Abstractions;
    using Models;

    [ExcludeFromCodeCoverage]
    public record CustomerAddressUpdated(Guid CustomerId, Address ShippingAddress) : IntegrationEvent;

}