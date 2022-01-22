using EventDriven.DependencyInjection.URF.Mongo;

namespace CustomerService.Configuration
{
    public class CustomerDatabaseSettings : IMongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
    }
}