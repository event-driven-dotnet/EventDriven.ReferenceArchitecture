using EventDriven.DependencyInjection.URF.Mongo;

namespace CustomerService.Configuration;

public class CustomerDatabaseSettings : IMongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string CollectionName { get; set; } = null!;
}