using EventDriven.DependencyInjection.URF.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OrderService.Configuration;
using OrderService.Domain.OrderAggregate;
using OrderService.Domain.OrderAggregate.CommandHandlers;
using OrderService.Integration.EventHandlers;
using OrderService.Repositories;

namespace OrderService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderService", Version = "v1" });
            });

            // Registrations
            services.AddAutoMapper(typeof(Startup));
            services.AddSingleton<OrderCommandHandler>();
            services.AddSingleton<IOrderRepository, OrderRepository>();
            services.AddMongoDbSettings<OrderDatabaseSettings, Order>(Configuration);
            services.AddSingleton<CustomerAddressUpdatedEventHandler>();

            // Add Dapr event bus
            services.AddDaprEventBus(Configuration, true);
            
            // Add Dapr Mongo event cache
            services.AddDaprMongoEventCache(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            CustomerAddressUpdatedEventHandler customerAddressUpdatedEventHandler)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CustomerService v1"));
            }

            app.UseRouting();
            
            // Use cloud events
            app.UseCloudEvents();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                
                // Map subscribe handlers
                endpoints.MapSubscribeHandler();
                endpoints.MapDaprEventBus(eventBus =>
                {
                    // Subscribe with event handler
                    eventBus.Subscribe(customerAddressUpdatedEventHandler, null, "v1");
                });
            });
        }
    }
}
