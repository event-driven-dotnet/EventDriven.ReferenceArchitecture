using EventDriven.DependencyInjection.URF.Mongo;

namespace OrderService.Configuration;

public class OrderDatabaseSettings : IMongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string CollectionName { get; set; } = null!;
}