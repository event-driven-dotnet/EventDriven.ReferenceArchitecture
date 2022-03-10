using AutoMapper;
using OrderService.Domain.OrderAggregate;
using Address = OrderService.Domain.OrderAggregate.Address;
using IntegrationAddress = Common.Integration.Models.Address;

namespace OrderService.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Order, OrderService.DTO.Write.Order>();
        CreateMap<Order, OrderService.DTO.Write.Order>().ReverseMap();
        CreateMap<Address, OrderService.DTO.Write.Address>();
        CreateMap<Address, OrderService.DTO.Write.Address>().ReverseMap();
        CreateMap<OrderItem, OrderService.DTO.Write.OrderItem>();
        CreateMap<OrderItem, OrderService.DTO.Write.OrderItem>().ReverseMap();
        CreateMap<OrderState, OrderService.DTO.Write.OrderState>();
        CreateMap<OrderState, OrderService.DTO.Write.OrderState>().ReverseMap();
        CreateMap<Address, IntegrationAddress>();
        CreateMap<Address, IntegrationAddress>().ReverseMap();
    }
}