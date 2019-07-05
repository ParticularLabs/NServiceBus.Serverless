using NServiceBus;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Transport;

namespace ServerlessTransportSpike
{
    public static class BaseTransportConfigurationExtensions
    {
        public static TransportExtensions<TBaseTransport> BaseTransportConfiguration<TBaseTransport>(
            this TransportExtensions<ServerlessTransport<TBaseTransport>> serverlessConfig) where TBaseTransport : TransportDefinition, new()
        {
            return new TransportExtensions<TBaseTransport>(serverlessConfig.GetSettings());
        }
    }
}