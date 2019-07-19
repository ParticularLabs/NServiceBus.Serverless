namespace AzureFunctionsDemo.Configuration
{
    using NServiceBus;
    using NServiceBus.Transport;

    public interface IDispatcherConfiguration
    {
        TransportDefinition Create(EndpointConfiguration configuration);
    }
}