using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using OrderService.Configuration;
using OrderService.Domain.OrderAggregate;
using OrderService.Domain.OrderAggregate.CommandHandlers;
using OrderService.Integration.EventHandlers;
using OrderService.Repositories;
using URF.Core.Abstractions;
using URF.Core.Mongo;

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

            // Configuration
            services.Configure<OrderDatabaseSettings>(
                Configuration.GetSection(nameof(OrderDatabaseSettings)));
            services.AddSingleton<IOrderDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<OrderDatabaseSettings>>().Value);

            // Registrations
            services.AddAutoMapper(typeof(Startup));
            services.AddSingleton<OrderCommandHandler>();
            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOrderDatabaseSettings>();
                var client = new MongoClient(settings.ConnectionString);
                return client.GetDatabase(settings.DatabaseName);
            });
            services.AddSingleton(sp =>
            {
                var context = sp.GetRequiredService<IMongoDatabase>();
                var settings = sp.GetRequiredService<IOrderDatabaseSettings>();
                return context.GetCollection<Order>(settings.OrdersCollectionName);
            });
            services.AddSingleton<IDocumentRepository<Order>, DocumentRepository<Order>>();
            services.AddSingleton<IOrderRepository, OrderRepository>();
            services.AddSingleton<CustomerAddressUpdatedEventHandler>();

            // Event Bus
            services.AddDaprEventBus(Constants.DaprPubSubName);
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
