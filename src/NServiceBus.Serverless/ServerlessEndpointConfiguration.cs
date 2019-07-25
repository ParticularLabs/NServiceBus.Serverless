namespace NServiceBus.Serverless
{
    using System;
    using Recoverability;
    using Serialization;
    using Transport;

    /// <summary>
    /// The configuration for an NServiceBus endpoint optimized for serverless environments.
    /// </summary>
    public class ServerlessEndpointConfiguration
    {
        /// <summary>
        /// Creates a new configuration.
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
        /// Configures the amount of times a message should be retried immediately when it fails. After exceeding the number of
        /// retries, the failure will be throw back to the caller.
        /// </summary>
        public void InMemoryRetries(int numberOfRetries)
        {
            EndpointConfiguration.Recoverability().Immediate(c => c.NumberOfRetries(numberOfRetries));
        }

        /// <summary>
        /// Moves a failed message to the error queue instead of throwing the exception of a failed message back to the caller.
        /// <c>false</c> by default.
        /// </summary>
        public void SendFailedMessagesToErrorQueue(bool sendFailedMessagesToErrorQueue = true)
        {
            recoverabilityPolicy.SendFailedMessagesToErrorQueue = sendFailedMessagesToErrorQueue;
        }

        /// <summary>
        /// Gives access to the underlying endpoint configuration for advanced configuration options.
        /// </summary>
        public void AdvancedConfiguration(Action<EndpointConfiguration> advancedConfiguration)
        {
            //allow access to the underlying EndpointConfiguration for all sorts of configurations/workarounds
            advancedConfiguration(EndpointConfiguration);
        }

        readonly ServerlessRecoverabilityPolicy recoverabilityPolicy = new ServerlessRecoverabilityPolicy();
    }
}