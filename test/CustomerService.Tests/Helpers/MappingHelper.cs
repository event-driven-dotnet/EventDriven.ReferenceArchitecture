using AutoMapper;
using CustomerService.Mapping;

namespace CustomerService.Tests.Helpers;

public static class MappingHelper
{
    public static IMapper GetMapper()
    {
        var config = new MapperConfiguration(
            cfg => { cfg.AddProfile(typeof(AutoMapperProfile)); });

        return config.CreateMapper();
    }
}