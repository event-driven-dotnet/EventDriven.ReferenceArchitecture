using System;
using System.Collections.Generic;
using CustomerService.Domain.CustomerAggregate;

namespace CustomerService.Tests.Helpers;

public class CustomerComparer : IEqualityComparer<Customer>
{
    public bool Equals(Customer? x, Customer? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.FirstName == y.FirstName && x.LastName == y.LastName && x.ShippingAddress.Equals(y.ShippingAddress);
    }

    public int GetHashCode(Customer obj)
    {
        return HashCode.Combine(obj.FirstName, obj.LastName, obj.ShippingAddress);
    }
}