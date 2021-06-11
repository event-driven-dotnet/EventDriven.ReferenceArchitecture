namespace OrderService.Configuration {

    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class OrderDatabaseSettings {

        public string OrdersCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

    }

}