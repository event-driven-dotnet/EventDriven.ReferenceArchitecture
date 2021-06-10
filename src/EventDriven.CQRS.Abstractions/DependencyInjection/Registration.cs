namespace EventDriven.CQRS.Abstractions.DependencyInjection {

    using System.Linq;
    using System.Reflection;
    using Commands;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    
    /// <summary>
    /// For registration of dependencies.
    /// </summary>
    public static class Registration {

        /// <summary>
        /// Register required dependencies with the service provider
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies">Assemblies which contain command definitions</param>
        /// <returns></returns>
        public static IServiceCollection AddCqrs(this IServiceCollection services, params Assembly[] assemblies) =>
            services.AddScoped<ICommandBroker, CommandBroker>()
                    .AddMediatR(assemblies);

    }

}