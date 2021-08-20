using AutoMapper;
using Entity = CustomerService.Domain.CustomerAggregate;
using Integration = Common.Integration.Models;

namespace CustomerService.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Entity.Customer, DTO.Write.Customer>();
            CreateMap<Entity.Customer, DTO.Write.Customer>().ReverseMap();
            CreateMap<Entity.Address, DTO.Write.Address>();
            CreateMap<Entity.Address, DTO.Write.Address>().ReverseMap();

            CreateMap<Entity.Customer, DTO.Read.CustomerView>().IncludeMembers(c => c.ShippingAddress);
            CreateMap<Entity.Customer, DTO.Read.CustomerView>().IncludeMembers(c => c.ShippingAddress).ReverseMap();
            CreateMap<Entity.Address, DTO.Read.CustomerView>();
            CreateMap<Entity.Address, DTO.Read.CustomerView>().ReverseMap();
            
            CreateMap<Entity.Address, Integration.Address>();
            CreateMap<Entity.Address, Integration.Address>().ReverseMap();
        }
    }
}