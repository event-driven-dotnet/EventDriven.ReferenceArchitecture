using CustomerService.DTO.Write;

namespace EventDriven.ReferenceArchitecture.Specs.Helpers;

public class CustomerWriteDtoAddressComparer : IEqualityComparer<Address>
{
    public bool Equals(Address? x, Address? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Street == y.Street
            && x.City == y.City
            && x.Country == y.Country
            && x.PostalCode == y.PostalCode;
    }

    public int GetHashCode(Address obj) =>
        HashCode.Combine(obj.Street, obj.City, obj.Country, obj.State, obj.PostalCode);
}