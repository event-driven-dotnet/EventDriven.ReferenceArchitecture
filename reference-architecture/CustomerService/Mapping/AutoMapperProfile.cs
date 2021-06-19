using AutoMapper;
using CustomerService.DTO.Read;
using CustomerService.DTO.Write;
using Entity = CustomerService.Domain.CustomerAggregate;
using Integration = Common.Integration.Models;

namespace CustomerService.Mapping
{

    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            CreateMap<Entity.Customer, Customer>();
            CreateMap<Entity.Customer, Customer>()
               .ReverseMap();
            CreateMap<Entity.Address, Address>();
            CreateMap<Entity.Address, Address>()
               .ReverseMap();

            CreateMap<Entity.Customer, CustomerView>()
               .IncludeMembers(c => c.ShippingAddress);
            CreateMap<Entity.Customer, CustomerView>()
               .IncludeMembers(c => c.ShippingAddress)
               .ReverseMap();
            CreateMap<Entity.Address, CustomerView>();
            CreateMap<Entity.Address, CustomerView>()
               .ReverseMap();

            CreateMap<Entity.Address, Integration.Address>();
            CreateMap<Entity.Address, Integration.Address>()
               .ReverseMap();
        }

    }

}