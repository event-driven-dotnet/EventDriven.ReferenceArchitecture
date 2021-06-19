using AutoMapper;
using OrderService.Domain.OrderAggregate;
using IntegrationAddress = Common.Integration.Models.Address;

namespace OrderService.Mapping
{

    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            CreateMap<Order, DTO.Write.Order>();
            CreateMap<Order, DTO.Write.Order>()
               .ReverseMap();
            CreateMap<Address, DTO.Write.Address>();
            CreateMap<Address, DTO.Write.Address>()
               .ReverseMap();
            CreateMap<OrderItem, DTO.Write.OrderItem>();
            CreateMap<OrderItem, DTO.Write.OrderItem>()
               .ReverseMap();
            CreateMap<OrderState, DTO.Write.OrderState>();
            CreateMap<OrderState, DTO.Write.OrderState>()
               .ReverseMap();
            CreateMap<Address, IntegrationAddress>();
            CreateMap<Address, IntegrationAddress>()
               .ReverseMap();
        }

    }

}