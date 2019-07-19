namespace AzureFunctionsDemo.Configuration
{
    using System;
    using NServiceBus;
    using NServiceBus.Configuration.AdvancedExtensibility;
    using NServiceBus.Transport;

    public class AzureServiceBusDispatcher : IDispatcherConfiguration
    {
        readonly string connectionString;
        readonly Action<RoutingSettings<AzureServiceBusTransport>> routingConfiguration;

        public AzureServiceBusDispatcher(string connectionString, Action<RoutingSettings<AzureServiceBusTransport>> routingConfiguration = null)
        {
            this.connectionString = connectionString;
            this.routingConfiguration = routingConfiguration;
        }

        public TransportDefinition Create(EndpointConfiguration configuration)
        {
            //TODO can we new up all our transports publicly?
            var asbConfig = new TransportExtensions<AzureServiceBusTransport>(configuration.GetSettings());
            asbConfig.ConnectionString(this.connectionString);
            var routing = asbConfig.Routing();
            routingConfiguration?.Invoke(routing);

            return new AzureServiceBusTransport();
        }
    }
}