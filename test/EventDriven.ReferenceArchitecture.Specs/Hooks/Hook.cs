using System.Diagnostics;
using BoDi;
using CustomerService.Configuration;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Repositories;
using EventDriven.DependencyInjection;
using EventDriven.DependencyInjection.URF.Mongo;
using EventDriven.ReferenceArchitecture.Specs.Configuration;
using EventDriven.ReferenceArchitecture.Specs.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderService.Configuration;
using OrderService.Domain.OrderAggregate;
using OrderService.Repositories;

namespace EventDriven.ReferenceArchitecture.Specs.Hooks;

[Binding]
public class Hooks
{
    private Process? _tyeProcess;
    private readonly IObjectContainer _objectContainer;
    private const string TyeArguments = "run" + " ../../..";

    public Hooks(IObjectContainer objectContainer)
    {
        _objectContainer = objectContainer;
    }

    [BeforeScenario]
    public async Task RegisterServices()
    {
        var host = Host
            .CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                var config = services.BuildServiceProvider()
                    .GetRequiredService<IConfiguration>();
                services.AddAppSettings<ReferenceArchSpecsSettings>(config);
                services.AddHttpClient();
                services.AddSingleton<ICustomerRepository, CustomerRepository>();
                services.AddSingleton<IOrderRepository, OrderRepository>();
                services.AddMongoDbSettings<CustomerDatabaseSettings, Customer>(config);
                services.AddMongoDbSettings<OrderDatabaseSettings, Order>(config);
            })
            .Build();

        var settings = host.Services.GetRequiredService<ReferenceArchSpecsSettings>();
        var customerRepository = host.Services.GetRequiredService<ICustomerRepository>();
        var orderRepository = host.Services.GetRequiredService<IOrderRepository>();
        var httpClientFactory = host.Services.GetRequiredService<IHttpClientFactory>();
        
        if (settings.StartTyeProcess)
            await StartTyeProcess(settings.TyeProcessTimeout);

        await ClearData(customerRepository, settings.Customer1Id);
        await ClearData(customerRepository, settings.Customer2Id);
        await ClearData(customerRepository, settings.Customer3Id);
        await ClearData(customerRepository, settings.CustomerPubSub1Id);
        await ClearData(orderRepository, settings.Order1Id);
        await ClearData(orderRepository, settings.Order2Id);
        await ClearData(orderRepository, settings.Order3Id);
        await ClearData(orderRepository, settings.Order4Id);
        await ClearData(orderRepository, settings.OrderPubSub1Id);
        await ClearData(orderRepository, settings.OrderPubSub2Id);
        
        _objectContainer.RegisterInstanceAs(settings);
        _objectContainer.RegisterInstanceAs(httpClientFactory);
        _objectContainer.RegisterInstanceAs(new JsonFilesRepository());
        _objectContainer.RegisterInstanceAs(customerRepository);
        _objectContainer.RegisterInstanceAs(orderRepository);
    }

    [AfterScenario]
    public void CleanUp()
    {
        try
        {
            if (_tyeProcess is { HasExited: false })
                _tyeProcess.Kill();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private async Task ClearData<TRepository>(TRepository repository, Guid entityId)
    {
        if (repository is ICustomerRepository customerRepository)
            await customerRepository.RemoveAsync(entityId);
        if (repository is IOrderRepository orderRepository)
            await orderRepository.RemoveAsync(entityId);
    }

    private async Task StartTyeProcess(TimeSpan waitForTyeProcess)
    {
        var processInfo = new ProcessStartInfo("tye", TyeArguments)
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true
        };
        _tyeProcess = Process.Start(processInfo);
        await Task.Delay(waitForTyeProcess);
    }
}