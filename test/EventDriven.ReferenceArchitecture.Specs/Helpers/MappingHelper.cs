using AutoMapper;

namespace EventDriven.ReferenceArchitecture.Specs.Helpers;

public static class MappingHelper
{
    public static IMapper GetMapper<TMappingProfile>()
        where TMappingProfile : Profile
    {
        var config = new MapperConfiguration(
            cfg => { cfg.AddProfile(typeof(TMappingProfile)); });

        return config.CreateMapper();
    }
}