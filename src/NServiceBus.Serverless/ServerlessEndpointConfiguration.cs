using System;
using System.Threading.Tasks;
using NServiceBus.Container;
using NServiceBus.DataBus;
using NServiceBus.ObjectBuilder;
using NServiceBus.Persistence;
using NServiceBus.Serialization;
using NServiceBus.Transport;

namespace NServiceBus.Serverless
{
    /// <summary>
    ///
    /// </summary>
    public abstract class ServerlessEndpointConfiguration
    {
        /// <summary>
        ///
        /// </summary>
        protected readonly EndpointConfiguration endpointConfiguration;

        /// <summary>
        ///
        /// </summary>
        protected IPipelineInvoker invoker;

        /// <summary>
        ///
        /// </summary>
        /// <param name="endpointName"></param>
        protected ServerlessEndpointConfiguration(string endpointName)
        {
            endpointConfiguration = new EndpointConfiguration(endpointName);
            endpointConfiguration.Recoverability().Delayed(settings => settings.NumberOfRetries(0));
            endpointConfiguration.DefineCriticalErrorAction(context =>
            {
                throw context.Exception;
            });
            endpointConfiguration.CustomDiagnosticsWriter(customDiagnosticsWriter);
            endpointConfiguration.SendFailedMessagesTo(SettingsKeys.ErrorQueueDisabled);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TDispatchTransport"></typeparam>
        /// <returns></returns>
        public TransportExtensions<TDispatchTransport> UseTransportForDispatch<TDispatchTransport>() where TDispatchTransport : TransportDefinition, new()
        {
            var serverlessTransportConfiguration = endpointConfiguration.UseTransport<ServerlessTransport<TDispatchTransport>>();

            serverlessTransportConfiguration.UseInMemoryRetries(UseInMemoryRetries);

            invoker = serverlessTransportConfiguration.PipelineAccess();

            return serverlessTransportConfiguration.BaseTransportConfiguration();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="immediateRetries"></param>
        /// <param name="errorQueue"></param>
        public void UseRecoverability(int immediateRetries, string errorQueue)
        {
            endpointConfiguration.Recoverability().Immediate(settings => settings.NumberOfRetries(immediateRetries));
            endpointConfiguration.SendFailedMessagesTo(errorQueue);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SerializationExtensions<T> AddDeserializer<T>() where T : SerializationDefinition, new() => endpointConfiguration.AddDeserializer<T>();

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddHeaderToAllOutgoingMessages(string key, string value) => endpointConfiguration.AddHeaderToAllOutgoingMessages(key, value);

        /// <summary>
        ///
        /// </summary>
        public AssemblyScannerConfiguration AssemblyScanner() => endpointConfiguration.AssemblyScanner();

        /// <summary>
        ///
        /// </summary>
        /// <param name="auditQueue"></param>
        /// <param name="timeToBeReceived"></param>
        public void AuditProcessedMessagesTo(string auditQueue, TimeSpan? timeToBeReceived = null) => endpointConfiguration.AuditProcessedMessagesTo(auditQueue, timeToBeReceived);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public ConventionsBuilder Conventions() => endpointConfiguration.Conventions();

        /// <summary>
        ///
        /// </summary>
        /// <param name="customStrategy"></param>
        public void CustomConversationIdStrategy(Func<ConversationIdStrategyContext, ConversationId> customStrategy) => endpointConfiguration.CustomConversationIdStrategy(customStrategy);

        /// <summary>
        ///
        /// </summary>
        /// <param name="address"></param>
        public void ForwardReceivedMessagesTo(string address) => endpointConfiguration.ForwardReceivedMessagesTo(address);

        /// <summary>
        ///
        /// </summary>
        /// <param name="licenseText"></param>
        public void License(string licenseText) => endpointConfiguration.License(licenseText);

        /// <summary>
        ///
        /// </summary>
        /// <param name="licenseFile"></param>
        public void LicensePath(string licenseFile) => endpointConfiguration.LicensePath(licenseFile);

        /// <summary>
        ///
        /// </summary>
        /// <param name="baseInputQueueName"></param>
        public void OverrideLocalAddress(string baseInputQueueName) => endpointConfiguration.OverrideLocalAddress(baseInputQueueName);

        /// <summary>
        ///
        /// </summary>
        /// <param name="address"></param>
        public void OverridePublicReturnAddress(string address) => endpointConfiguration.OverridePublicReturnAddress(address);

        /// <summary>
        ///
        /// </summary>
        public Pipeline.PipelineSettings Pipeline => endpointConfiguration.Pipeline;

        /// <summary>
        ///
        /// </summary>
        /// <param name="registration"></param>
        public void RegisterComponents(Action<IConfigureComponents> registration) => endpointConfiguration.RegisterComponents(registration);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public UnitOfWorkSettings UnitOfWork() => endpointConfiguration.UnitOfWork();

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="customizations"></param>
        public void UseContainer<T>(Action<ContainerCustomizations> customizations) where T: ContainerDefinition, new() => endpointConfiguration.UseContainer<T>(customizations);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DataBusExtensions<T> UseDataBus<T>() where T: DataBusDefinition, new() => endpointConfiguration.UseDataBus<T>();

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public PersistenceExtensions<T> UsePersistence<T>() where T : PersistenceDefinition, new() => endpointConfiguration.UsePersistence<T>();

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SerializationExtensions<T> UseSerialization<T>() where T : SerializationDefinition, new() => endpointConfiguration.UseSerialization<T>();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        protected Task Initialize()
        {
            return Endpoint.Start(endpointConfiguration);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected abstract Task customDiagnosticsWriter(string arg);

        /// <summary>
        ///
        /// </summary>
        protected abstract bool UseInMemoryRetries { get; }
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ServerlessEndpointConfiguration<T> : ServerlessEndpointConfiguration
    {
        private readonly INativeMessageProcessor<T> processor;

        /// <summary>
        ///
        /// </summary>
        /// <param name="endpointName"></param>
        /// <param name="processor"></param>
        public ServerlessEndpointConfiguration(string endpointName, INativeMessageProcessor<T> processor) : base(endpointName)
        {
            this.processor = processor;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public new async Task<IServerlessEndpointInstance<T>> Initialize()
        {
            await base.Initialize().ConfigureAwait(false);
            return new ServerlessEndpointInstance<T>(invoker, processor);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public abstract class ServerlessEndpointConfiguration<T1, T2> : ServerlessEndpointConfiguration
    {
        private readonly INativeMessageProcessor<T1, T2> processor;

        /// <summary>
        ///
        /// </summary>
        /// <param name="endpointName"></param>
        /// <param name="processor"></param>
        public ServerlessEndpointConfiguration(string endpointName, INativeMessageProcessor<T1, T2> processor) : base(endpointName)
        {
            this.processor = processor;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public new async Task<IServerlessEndpointInstance<T1, T2>> Initialize()
        {
            await base.Initialize().ConfigureAwait(false);
            return new ServerlessEndpointInstance<T1, T2>(invoker, processor);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public abstract class ServerlessEndpointConfiguration<T1, T2, T3> : ServerlessEndpointConfiguration
    {
        private readonly INativeMessageProcessor<T1, T2, T3> processor;

        /// <summary>
        ///
        /// </summary>
        /// <param name="endpointName"></param>
        /// <param name="processor"></param>
        public ServerlessEndpointConfiguration(string endpointName, INativeMessageProcessor<T1, T2, T3> processor) : base(endpointName)
        {
            this.processor = processor;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public new async Task<IServerlessEndpointInstance<T1, T2, T3>> Initialize()
        {
            await base.Initialize().ConfigureAwait(false);
            return new ServerlessEndpointInstance<T1, T2, T3>(invoker, processor);
        }
    }
}
