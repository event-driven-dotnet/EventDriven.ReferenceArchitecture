using EventDriven.DependencyInjection.URF.Mongo;

namespace OrderService.Configuration
{
    public class OrderDatabaseSettings : IMongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
    }
}