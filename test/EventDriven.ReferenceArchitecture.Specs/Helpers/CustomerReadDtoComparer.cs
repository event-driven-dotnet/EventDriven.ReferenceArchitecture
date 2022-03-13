using CustomerService.DTO.Read;

namespace EventDriven.ReferenceArchitecture.Specs.Helpers;

public class CustomerReadDtoComparer : IEqualityComparer<CustomerView>
{
    public bool Equals(CustomerView? x, CustomerView? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Id == y.Id
               && x.FirstName == y.FirstName
               && x.LastName == y.LastName
               && x.Street == y.Street
               && x.City == y.City
               && x.State == y.State
               && x.Country == y.Country
               && x.PostalCode == y.PostalCode;
    }

    public int GetHashCode(CustomerView obj) =>
        HashCode.Combine(obj.FirstName, obj.LastName, obj.Street, obj.City, obj.State, obj.Country, obj.PostalCode);
}