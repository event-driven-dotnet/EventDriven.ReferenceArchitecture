using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using AutoMapper;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Repositories;
using EventDriven.ReferenceArchitecture.Specs.Configuration;
using EventDriven.ReferenceArchitecture.Specs.Helpers;
using EventDriven.ReferenceArchitecture.Specs.Repositories;
using OrderService.Domain.OrderAggregate;
using OrderService.Repositories;
using TechTalk.SpecFlow.Assist;
using Xunit;
using CustomerWriteDto = CustomerService.DTO.Write.Customer;
using CustomerReadDto = CustomerService.DTO.Read.CustomerView;
using OrderWriteDto = OrderService.DTO.Write.Order;
using OrderReadDto = OrderService.DTO.Read.OrderView;
using OrderWriteDtoAddress = OrderService.DTO.Write.Address;
using CustomerMappingProfile = CustomerService.Mapping.AutoMapperProfile;
using OrderMappingProfile = OrderService.Mapping.AutoMapperProfile;

namespace EventDriven.ReferenceArchitecture.Specs.Steps;

[Binding]
[CollectionDefinition("SpecFlowNonParallelizableFeatures", DisableParallelization = true)]
public class StepDefinitions
{
    private IMapper OrderMapper { get; }
    private ReferenceArchSpecsSettings SpecSettings { get; }
    private HttpClient CustomerClient { get; }
    private HttpClient OrderClient { get; }
    private ICustomerRepository CustomerRepository { get; }
    private IOrderRepository OrderRepository { get; }
    private JsonFilesRepository JsonFilesRepo { get; }
    private HttpResponseMessage Response { get; set; } = null!;
    private JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true
    };

    public StepDefinitions(
        ReferenceArchSpecsSettings specSettings,
        IHttpClientFactory factory,
        ICustomerRepository customerRepository,
        IOrderRepository orderRepository,
        JsonFilesRepository jsonFilesRepo)
    {
        SpecSettings = specSettings;
        CustomerClient = factory.CreateClient();
        CustomerClient.BaseAddress = new Uri(SpecSettings.CustomerServiceBaseAddress!);
        OrderClient = factory.CreateClient();
        OrderClient.BaseAddress = new Uri(SpecSettings.OrderServiceBaseAddress!);
        CustomerRepository = customerRepository;
        OrderRepository = orderRepository;
        JsonFilesRepo = jsonFilesRepo;
        OrderMapper = MappingHelper.GetMapper<OrderMappingProfile>();
    }
    
    [Given(@"customers have been created with '(.*)'")]
    public async Task GivenCustomersHaveBeenCreated(string file)
    {
        var json = JsonFilesRepo.Files[file];
        var customers = JsonSerializer.Deserialize<IList<Customer>>(json, JsonSerializerOptions);
        foreach (var customer in customers!)
        {
            if (customer.Id == SpecSettings.Customer2Id)
                await CustomerRepository.AddAsync(customer);
            if (customer.Id == SpecSettings.Customer3Id)
                await CustomerRepository.AddAsync(customer);
        }
    }

    [Given(@"a customer has been created with '(.*)'")]
    public async Task  GivenACustomerHasBeenCreatedWith(string file)
    {
        var json = JsonFilesRepo.Files[file];
        var customer = JsonSerializer.Deserialize<Customer>(json, JsonSerializerOptions);
        if (customer != null)
            await CustomerRepository.AddAsync(customer);
    }

    [Given(@"orders have been created with '(.*)'")]
    public async Task  GivenOrdersHaveBeenCreatedWith(string file)
    {
        var ordersJson = JsonFilesRepo.Files[file];
        var orders = JsonSerializer.Deserialize<List<Order>>(ordersJson, JsonSerializerOptions);
        foreach (var order in orders!)
            await OrderRepository.AddAsync(order);
    }

    [Given(@"an order has been created with '(.*)'")]
    public async Task GivenAnOrderHasBeenCreatedWith(string file)
    {
        var json = JsonFilesRepo.Files[file];
        var order = JsonSerializer.Deserialize<Order>(json, JsonSerializerOptions);
        if (order != null)
            await OrderRepository.AddAsync(order);
    }

    [When(@"I make a GET request for '(.*)' to '(.*)'")]
    public async Task WhenIMakeAgetRequestTo(HttpClientType clientType, string endpoint)
    {
        Response = await GetHttpClient(clientType).GetAsync(endpoint);
    }

    [When(@"I make a POST request for '(.*)' with '(.*)' to '(.*)'")]
    public async Task WhenIMakeApostRequestWithTo(HttpClientType clientType, string file, string endpoint)
    {
        var json = JsonFilesRepo.Files[file];
        var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        Response = await GetHttpClient(clientType).PostAsync(endpoint, content);
    }

    [When(@"I make a PUT request for '(.*)' with '(.*)' to '(.*)'")]
    public async Task  WhenIMakeAPutRequestWithTo(HttpClientType clientType, string file, string endpoint)
    {
        var json = JsonFilesRepo.Files[file];
        var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        Response = await GetHttpClient(clientType).PutAsync(endpoint, content);
    }

    [When(@"I make a PUT request for '(.*)' with the following data to '(.*)'")]
    public async Task WhenIMakeAputRequestWithTheFollowingData(HttpClientType clientType, string endpoint, Table table)
    {
        var request = table.CreateInstance<PutRequest>();
        Response = await GetHttpClient(clientType).PutAsync($"{endpoint}/{request.Id}/{request.ETag}", null);
    }

    [When(@"I make a DELETE request for '(.*)' with id '(.*)' to '(.*)'")]
    public async Task WhenIMakeAdeleteRequestWithIdTo(HttpClientType clientType, string id, string endpoint)
    {
        Response = await GetHttpClient(clientType).DeleteAsync($"{endpoint}/{id}");
    }

    [Then(@"the response status code should be '(.*)'")]
    public void ThenTheResponseStatusCodeShouldBe(int statusCode)
    {
        var expected = (HttpStatusCode)statusCode;
        Assert.Equal(expected, Response.StatusCode);
    }

    [Then(@"the location header should be '(.*)'")]
    public void ThenTheLocationHeaderIs(Uri location)
    {
        Assert.Equal(location, Response.Headers.Location);
    }

    [Then(@"the response customers-view should be '(.*)'")]
    public async Task ThenTheResponseCustomeraShouldBe(string file)
    {
        var comparer = new CustomerReadDtoComparer();
        var json = JsonFilesRepo.Files[file];
        var expectedList = JsonSerializer.Deserialize<List<CustomerReadDto>>(json, JsonSerializerOptions);
        var actualList = await Response.Content.ReadFromJsonAsync<List<CustomerReadDto>>();
        var expected1 = expectedList!.Single(e => e.Id == SpecSettings.Customer2Id);
        var expected2 = expectedList!.Single(e => e.Id == SpecSettings.Customer3Id);
        var actual1 = actualList!.Single(e => e.Id == SpecSettings.Customer2Id);
        var actual2 = actualList!.Single(e => e.Id == SpecSettings.Customer3Id);
        Assert.Equal(expected1, actual1, comparer);
        Assert.Equal(expected2, actual2, comparer);
    }

    [Then(@"the response orders-view should be '(.*)'")]
    public async Task ThenTheResponseOrdersViewShouldBe(string file)
    {
        var comparer = new OrderReadDtoComparer();
        var json = JsonFilesRepo.Files[file];
        var expectedList = JsonSerializer.Deserialize<List<OrderReadDto>>(json, JsonSerializerOptions);
        var actualList = await Response.Content.ReadFromJsonAsync<List<OrderReadDto>>();
        var expected1 = expectedList!.Single(e => e.Id == SpecSettings.Order1Id);
        var expected2 = expectedList!.Single(e => e.Id == SpecSettings.Order2Id);
        var actual1 = actualList!.Single(e => e.Id == SpecSettings.Order1Id);
        var actual2 = actualList!.Single(e => e.Id == SpecSettings.Order2Id);
        Assert.Equal(expected1, actual1, comparer);
        Assert.Equal(expected2, actual2, comparer);
    }

    [Then(@"the response customer should be '(.*)'")]
    public async Task ThenTheResponseCustomerShouldBe(string file)
    {
        var json = JsonFilesRepo.Files[file];
        var expected = JsonSerializer.Deserialize<CustomerWriteDto>(json, JsonSerializerOptions);
        var actual = await Response.Content.ReadFromJsonAsync<CustomerWriteDto>();
        Assert.Equal(expected, actual, new CustomerWriteDtoComparer()!);
    }

    [Then(@"the response customer-view should be '(.*)'")]
    public async Task ThenTheResponseCustomerViewShouldBe(string file)
    {
        var json = JsonFilesRepo.Files[file];
        var expected = JsonSerializer.Deserialize<CustomerReadDto>(json, JsonSerializerOptions);
        var actual = await Response.Content.ReadFromJsonAsync<CustomerReadDto>();
        Assert.Equal(expected, actual, new CustomerReadDtoComparer()!);
    }

    [Then(@"the response order-view should be '(.*)'")]
    public async Task ThenTheResponseOrderViewShouldBe(string file)
    {
        var json = JsonFilesRepo.Files[file];
        var expected = JsonSerializer.Deserialize<OrderReadDto>(json, JsonSerializerOptions);
        var actual = await Response.Content.ReadFromJsonAsync<OrderReadDto>();
        Assert.Equal(expected, actual, new OrderReadDtoComparer()!);
    }

    [Then(@"the address for orders should equal '(.*)'")]
    public async Task  ThenTheAddressForOrdersShouldEqual(string file)
    {
        await Task.Delay(SpecSettings.AddressUpdateTimeout);
        var json = JsonFilesRepo.Files[file];
        var address = JsonSerializer.Deserialize<OrderWriteDtoAddress>(json, JsonSerializerOptions);
        var orders = await OrderRepository.GetByCustomerAsync(SpecSettings.CustomerPubSub1Id);
        var ordersDto = OrderMapper.Map<List<OrderWriteDto>>(orders);
        foreach (var order in ordersDto)
            Assert.Equal(address, order.ShippingAddress, new OrderWriteDtoAddressComparer()!);
    }

    private HttpClient GetHttpClient(HttpClientType clientType)
    {
        switch (clientType)
        {
            case HttpClientType.Customer:
                return CustomerClient;
            case HttpClientType.Order:
                return OrderClient;
            default:
                throw new Exception("Unsupported HttpClientType.");
        }
    }
}

public enum HttpClientType
{
    Customer,
    Order
}
