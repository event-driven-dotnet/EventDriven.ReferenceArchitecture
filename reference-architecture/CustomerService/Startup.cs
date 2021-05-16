using CustomerService.Configuration;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Domain.CustomerAggregate.CommandHandlers;
using CustomerService.Repositories;
using EventDriven.EventBus.Dapr;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using URF.Core.Abstractions;
using URF.Core.Mongo;

namespace CustomerService
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CustomerService", Version = "v1" });
            });

            // Configuration
            services.Configure<CustomerDatabaseSettings>(
                Configuration.GetSection(nameof(CustomerDatabaseSettings)));
            services.AddSingleton(sp =>
                sp.GetRequiredService<IOptions<CustomerDatabaseSettings>>().Value);

            // Registrations
            services.AddAutoMapper(typeof(Startup));
            services.AddSingleton<CustomerCommandHandler>();
            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<CustomerDatabaseSettings>();
                var client = new MongoClient(settings.ConnectionString);
                var database = client.GetDatabase(settings.DatabaseName);
                return database.GetCollection<Customer>(settings.CustomersCollectionName);
            });
            services.AddSingleton<IDocumentRepository<Customer>, DocumentRepository<Customer>>();
            services.AddSingleton<ICustomerRepository, CustomerRepository>();

            // Configuration
            var eventBusOptions = new DaprEventBusOptions();
            Configuration.GetSection(nameof(DaprEventBusOptions)).Bind(eventBusOptions);
            var eventBusSchemaOptions = new DaprEventBusSchemaOptions();
            Configuration.GetSection(nameof(DaprEventBusSchemaOptions)).Bind(eventBusSchemaOptions);

            // Add Dapr event bus
            services.AddDaprEventBus(eventBusOptions.PubSubName, options =>
            {
                options.UseSchemaRegistry = eventBusSchemaOptions.UseSchemaRegistry;
                options.SchemaRegistryType = eventBusSchemaOptions.SchemaRegistryType;
                options.MongoStateStoreOptions = eventBusSchemaOptions.MongoStateStoreOptions;
                options.SchemaValidatorType = eventBusSchemaOptions.SchemaValidatorType;
                options.AddSchemaOnPublish = eventBusSchemaOptions.AddSchemaOnPublish;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CustomerService v1"));
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
