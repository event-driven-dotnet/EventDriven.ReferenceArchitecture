using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Repositories;
using EventDriven.ReferenceArchitecture.Specs.Configuration;
using EventDriven.ReferenceArchitecture.Specs.Helpers;
using EventDriven.ReferenceArchitecture.Specs.Repositories;
using OrderService.Domain.OrderAggregate;
using OrderService.Repositories;
using Xunit;
using Address = OrderService.Domain.OrderAggregate.Address;

namespace EventDriven.ReferenceArchitecture.Specs.Steps;

[Binding]
public class UpdateCustomerAddressStepDefinitions
{
    private ReferenceArchitectureSpecsSettings SpecSettings { get; }
    private HttpClient Client { get; }
    private ICustomerRepository CustomerRepository { get; }
    private IOrderRepository OrderRepository { get; }
    private JsonFilesRepository JsonFilesRepo { get; }
    private Customer? Customer { get; set; }
    private HttpResponseMessage Response { get; set; } = null!;
    private JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true
    };

    public UpdateCustomerAddressStepDefinitions(
        ReferenceArchitectureSpecsSettings specSettings,
        HttpClient client,
        ICustomerRepository customerRepository,
        IOrderRepository orderRepository,
        JsonFilesRepository jsonFilesRepo)
    {
        SpecSettings = specSettings;
        Client = client;
        CustomerRepository = customerRepository;
        OrderRepository = orderRepository;
        JsonFilesRepo = jsonFilesRepo;
    }
    
    [Given(@"a customer has been created with '(.*)'")]
    public async Task  GivenACustomerHasBeenCreatedWith(string file)
    {
        var customerJson = JsonFilesRepo.Files[file];
        Customer = JsonSerializer.Deserialize<Customer>(customerJson, JsonSerializerOptions);
        if (Customer != null)
            await CustomerRepository.AddAsync(Customer);
    }

    [Given(@"orders have been created with '(.*)'")]
    public async Task  GivenOrdersHaveBeenCreatedWith(string file)
    {
        var ordersJson = JsonFilesRepo.Files[file];
        var orders = JsonSerializer.Deserialize<List<Order>>(ordersJson, JsonSerializerOptions);
        foreach (var order in orders!)
            await OrderRepository.AddAsync(order);
    }

    [When(@"I make a PUT request with '(.*)' to '(.*)'")]
    public async Task  WhenIMakeAPutRequestWithTo(string file, string endpoint)
    {
        var json = JsonFilesRepo.Files[file];
        var customer = JsonSerializer.Deserialize<Customer>(json, JsonSerializerOptions);
        customer!.ETag = Customer?.ETag!;
        var customerJson = JsonSerializer.Serialize(customer);
        var content = new StringContent(customerJson, Encoding.UTF8, MediaTypeNames.Application.Json);
        Response = await Client.PutAsync(endpoint, content);
    }

    [Then(@"the response status code should be '(.*)'")]
    public void ThenTheResponseStatusCodeShouldBe(int statusCode)
    {
        var expected = (HttpStatusCode)statusCode;
        Assert.Equal(expected, Response.StatusCode);
    }

    [Then(@"the address for orders should equal '(.*)'")]
    public async Task  ThenTheAddressForOrdersShouldEqual(string file)
    {
        await Task.Delay(SpecSettings.AddressUpdateTimeout);
        var json = JsonFilesRepo.Files[file];
        var address = JsonSerializer.Deserialize<Address>(json, JsonSerializerOptions);
        var orders = await OrderRepository.GetByCustomerAsync(Customer!.Id);
        foreach (var order in orders)
            Assert.Equal(address, order.ShippingAddress, new AddressComparer()!);
    }
}