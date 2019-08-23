namespace NServiceBus.Serverless
{
    using Recoverability;
    using Serialization;
    using Transport;

    /// <summary>
    /// The configuration for an NServiceBus endpoint optimized for serverless environments.
    /// </summary>
    public abstract class ServerlessEndpointConfiguration
    {
        /// <summary>
        /// Creates a new configuration.
        /// </summary>
        protected ServerlessEndpointConfiguration(string endpointName)
        {
            EndpointConfiguration = new EndpointConfiguration(endpointName);

            EndpointConfiguration.UsePersistence<InMemoryPersistence>();

            //TODO: currently ServerLess transport has transaction mode NONE which disables immediate and delayed retries anyway
            //make sure a call to "onError" will move the message to the error queue.
            EndpointConfiguration.Recoverability().Delayed(c => c.NumberOfRetries(0));
            //bubble exception to the function by default
            recoverabilityPolicy.SendFailedMessagesToErrorQueue = false;
            EndpointConfiguration.Recoverability().CustomPolicy(recoverabilityPolicy.Invoke);

            //will be overwritten when configuring an actual dispatcher transport
            UseTransportForDispatch<DummyDispatcher>();
        }

        internal EndpointConfiguration EndpointConfiguration { get; }

        internal PipelineInvoker PipelineInvoker { get; private set; }

        /// <summary>
        /// Define a transport to be used when sending and publishing messages.
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
        /// Define the serializer to be used.
        /// </summary>
        public SerializationExtensions<T> UseSerialization<T>() where T : SerializationDefinition, new()
        {
            return EndpointConfiguration.UseSerialization<T>();
        }
        
        /// <summary>
        /// Moves a failed message to a configured error queue instead of throwing the exception of a failed message back to the caller.
        /// The default error queue name is `error`.
        /// </summary>
        public void SendFailedMessageToErrorQueue(string errorQueueName = "error")
        {
            recoverabilityPolicy.SendFailedMessagesToErrorQueue = true;
            EndpointConfiguration.SendFailedMessagesTo(errorQueueName);
        }
        
        /// <summary>
        /// Gives access to the underlying endpoint configuration for advanced configuration options.
        /// </summary>
        public EndpointConfiguration AdvancedConfiguration => EndpointConfiguration;

        readonly ServerlessRecoverabilityPolicy recoverabilityPolicy = new ServerlessRecoverabilityPolicy();
    }
}