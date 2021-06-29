using System.Diagnostics.CodeAnalysis;

namespace OrderService.Configuration
{
    [ExcludeFromCodeCoverage]
    public class OrderDatabaseSettings
    {
        public string OrdersCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}