namespace NServiceBus.Serverless
{
    using System;
    using Serialization;
    using Transport;

    /// <summary>
    /// 
    /// </summary>
    public class ServerlessEndpointConfiguration
    {
        internal EndpointConfiguration EndpointConfiguration { get; }

        internal PipelineInvoker PipelineInvoker { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        public ServerlessEndpointConfiguration(string endpointName)
        {
            EndpointConfiguration = new EndpointConfiguration(endpointName);

            EndpointConfiguration.UsePersistence<InMemoryPersistence>();

            //will be overwritten when configuring an actual dispatcher transport
            UseTransportForDispatch<DummyDispatcher>();
        }

        /// <summary>
        /// 
        /// </summary>
        public TransportExtensions<TTransport> UseTransportForDispatch<TTransport>() 
            where TTransport : TransportDefinition, new()
        {
            var serverlessTransport = EndpointConfiguration.UseTransport<ServerlessTransport<TTransport>>();
            //TODO improve
            PipelineInvoker = serverlessTransport.PipelineAccess();
            return serverlessTransport.BaseTransportConfiguration();
        }

        /// <summary>
        /// 
        /// </summary>
        public SerializationExtensions<T> UseSerialization<T>() where T : SerializationDefinition, new()
        {
            return EndpointConfiguration.UseSerialization<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void AdvancedConfiguration(Action<EndpointConfiguration> advancedConfiguration)
        {
            //allow access to the underlying EndpointConfiguration for all sorts of configurations/workarounds
            advancedConfiguration(EndpointConfiguration);
        }
    }
}