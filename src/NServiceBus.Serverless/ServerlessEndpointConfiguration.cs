namespace NServiceBus.Serverless
{
    using System;
    using Recoverability;
    using Serialization;
    using Transport;

    /// <summary>
    /// 
    /// </summary>
    public class ServerlessEndpointConfiguration
    {
        readonly ServerlessRecoverabilityPolicy recoverabilityPolicy = new ServerlessRecoverabilityPolicy();

        internal EndpointConfiguration EndpointConfiguration { get; }

        internal PipelineInvoker PipelineInvoker { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        public ServerlessEndpointConfiguration(string endpointName)
        {
            EndpointConfiguration = new EndpointConfiguration(endpointName);

            EndpointConfiguration.UsePersistence<InMemoryPersistence>();

            //TODO: currently ServerLess transport has transaction mode NONE which disables immediate and delayed retries anyway
            //make sure a call to "onError" will move the message to the error queue. In-memory retries are handled by the transport internally.
            EndpointConfiguration.Recoverability().Immediate(c => c.NumberOfRetries(0));
            EndpointConfiguration.Recoverability().Delayed(c => c.NumberOfRetries(0));
            //bubble exception to the function by default
            recoverabilityPolicy.SendFailedMessagesToErrorQueue = false;
            EndpointConfiguration.Recoverability().CustomPolicy(recoverabilityPolicy.Invoke);

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
        public void InMemoryRetries(int numberOfRetries)
        {
            EndpointConfiguration.Recoverability().Immediate(c => c.NumberOfRetries(numberOfRetries));
        }

        /// <summary>
        /// 
        /// </summary>
        public void SendFailedMessagesToErrorQueue()
        {
            recoverabilityPolicy.SendFailedMessagesToErrorQueue = true;
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