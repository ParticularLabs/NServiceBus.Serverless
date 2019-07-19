namespace AzureFunctionsDemo.Configuration
{
    using System;
    using NServiceBus;
    using NServiceBus.Configuration.AdvancedExtensibility;
    using NServiceBus.Transport;

    public class StorageQueueDispatcher : IDispatcherConfiguration
    {
        string connectionString;
        readonly Action<RoutingSettings<AzureStorageQueueTransport>> routing;

        public StorageQueueDispatcher(string connectionString, Action<RoutingSettings<AzureStorageQueueTransport>> routing)
        {
            this.connectionString = connectionString;
            this.routing = routing;
        }

        public TransportDefinition Create(EndpointConfiguration configuration)
        {
            var asqConfig = new TransportExtensions<AzureStorageQueueTransport>(configuration.GetSettings());
            asqConfig.ConnectionString(this.connectionString);
            routing(asqConfig.Routing());
            return new AzureStorageQueueTransport();
        }
    }
}