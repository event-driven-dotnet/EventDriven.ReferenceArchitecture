namespace CustomerService.Configuration {

    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class CustomerDatabaseSettings {

        public string CustomersCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

    }

}