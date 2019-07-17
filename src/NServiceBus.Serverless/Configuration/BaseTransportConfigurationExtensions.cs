using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Serverless;
using NServiceBus.Transport;

namespace NServiceBus
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