using CustomerService.DTO.Write;

namespace EventDriven.ReferenceArchitecture.Specs.Helpers;

public class CustomerWriteDtoComparer : IEqualityComparer<Customer>
{
    public bool Equals(Customer? x, Customer? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Id == y.Id
               && x.FirstName == y.FirstName
               && x.LastName == y.LastName
               && new CustomerWriteDtoAddressComparer().Equals(x.ShippingAddress, y.ShippingAddress);
    }

    public int GetHashCode(Customer obj) =>
        HashCode.Combine(obj.FirstName, obj.LastName, obj.ShippingAddress);
}