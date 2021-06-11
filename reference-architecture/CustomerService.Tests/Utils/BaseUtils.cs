namespace CustomerService.Tests.Utils {

    using AutoMapper;
    using Mapping;

    public static class BaseUtils {

        public static IMapper GetMapper() {
            var config = new MapperConfiguration(cfg => {
                                                     cfg.AddProfile(typeof(AutoMapperProfile));
                                                 });

            return config.CreateMapper();
        }

    }

}