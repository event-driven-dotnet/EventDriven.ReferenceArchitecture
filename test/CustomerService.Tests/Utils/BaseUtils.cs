using AutoMapper;
using CustomerService.Mapping;

namespace CustomerService.Tests.Utils;

public static class BaseUtils
{
    public static IMapper GetMapper()
    {
        var config = new MapperConfiguration(
            cfg => { cfg.AddProfile(typeof(AutoMapperProfile)); });

        return config.CreateMapper();
    }
}