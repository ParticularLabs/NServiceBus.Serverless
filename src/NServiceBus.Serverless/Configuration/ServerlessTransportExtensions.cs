using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Serverless;
using NServiceBus.Transport;

namespace NServiceBus
{
    /// <summary>
    /// Extensions for serverless transports
    /// </summary>
    public static class ServerlessTransportExtensions
    {
        /// <summary>
        /// Provides access to the PipelineInvoker
        /// </summary>
        /// <param name="transportConfiguration"></param>
        /// <returns></returns>
        public static PipelineInvoker PipelineAccess<TBaseTransport>(
            this TransportExtensions<ServerlessTransport<TBaseTransport>> transportConfiguration) where TBaseTransport : TransportDefinition, new()
        {
            return transportConfiguration.GetSettings().GetOrCreate<PipelineInvoker>();
        }

        /// <summary>
        /// Provides access to the base transport configuration options
        /// </summary>
        /// <typeparam name="TBaseTransport"></typeparam>
        /// <param name="transportConfiguration"></param>
        /// <returns></returns>
        public static TransportExtensions<TBaseTransport> BaseTransportConfiguration<TBaseTransport>(
            this TransportExtensions<ServerlessTransport<TBaseTransport>> transportConfiguration) where TBaseTransport : TransportDefinition, new()
        {
            return new TransportExtensions<TBaseTransport>(transportConfiguration.GetSettings());
        }
    }
}