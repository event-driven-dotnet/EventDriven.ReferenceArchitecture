using OrderService.DTO.Read;

namespace EventDriven.ReferenceArchitecture.Specs.Helpers;

public class OrderReadDtoComparer : IEqualityComparer<OrderView>
{
    public bool Equals(OrderView? x, OrderView? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Id == y.Id
               && x.CustomerId == y.CustomerId
               && x.OrderDate == y.OrderDate
               && x.OrderTotal == y.OrderTotal
               && x.Street == y.Street
               && x.City == y.City
               && x.State == y.State
               && x.Country == y.Country
               && x.PostalCode == y.PostalCode
               && x.OrderDate == y.OrderDate;
    }

    public int GetHashCode(OrderView obj) =>
        HashCode.Combine(obj.CustomerId, obj.OrderDate, obj.OrderTotal,
            obj.Street, obj.City, obj.State, obj.Country, obj.PostalCode);
}