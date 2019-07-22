using System;
using NServiceBus.Container;
using NServiceBus.DataBus;
using NServiceBus.ObjectBuilder;
using NServiceBus.Persistence;
using NServiceBus.Serialization;

namespace NServiceBus.Serverless
{
    public abstract partial class ServerlessEndpointConfiguration
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TSerializationDefinition"></typeparam>
        /// <returns></returns>
        public SerializationExtensions<TSerializationDefinition> AddDeserializer<TSerializationDefinition>() where TSerializationDefinition : SerializationDefinition, new() => endpointConfiguration.AddDeserializer<TSerializationDefinition>();

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
        /// <typeparam name="TContainerDefinition"></typeparam>
        /// <param name="customizations"></param>
        public void UseContainer<TContainerDefinition>(Action<ContainerCustomizations> customizations) where TContainerDefinition : ContainerDefinition, new() => endpointConfiguration.UseContainer<TContainerDefinition>(customizations);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TDataBus"></typeparam>
        /// <returns></returns>
        public DataBusExtensions<TDataBus> UseDataBus<TDataBus>() where TDataBus : DataBusDefinition, new() => endpointConfiguration.UseDataBus<TDataBus>();

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TPersistenceDefinition"></typeparam>
        /// <returns></returns>
        public PersistenceExtensions<TPersistenceDefinition> UsePersistence<TPersistenceDefinition>() where TPersistenceDefinition : PersistenceDefinition => endpointConfiguration.UsePersistence<TPersistenceDefinition>();

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TSerializerDefinition"></typeparam>
        /// <returns></returns>
        public SerializationExtensions<TSerializerDefinition> UseSerialization<TSerializerDefinition>() where TSerializerDefinition : SerializationDefinition, new() => endpointConfiguration.UseSerialization<TSerializerDefinition>();
    }
}
