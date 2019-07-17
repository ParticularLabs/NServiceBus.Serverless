using System;
using System.Collections.Generic;
using NServiceBus.Routing;
using NServiceBus.Settings;
using NServiceBus.Transport;

namespace NServiceBus.Serverless
{
    sealed class ServerlessTransportInfrastructure<TBaseInfrastructure> : TransportInfrastructure
    {
        private readonly TransportInfrastructure baseTransportInfrastructure;
        private readonly SettingsHolder settings;

        public ServerlessTransportInfrastructure(TransportInfrastructure baseTransportInfrastructure,
            SettingsHolder settings)
        {
            this.baseTransportInfrastructure = baseTransportInfrastructure;
            this.settings = settings;
        }

        public override TransportReceiveInfrastructure ConfigureReceiveInfrastructure()
        {
            var pipelineInvoker = settings.GetOrCreate<PipelineInvoker>();
            return new ManualPipelineInvocationInfrastructure(pipelineInvoker);
        }

        public override TransportSendInfrastructure ConfigureSendInfrastructure()
        {
            return baseTransportInfrastructure.ConfigureSendInfrastructure();
        }

        public override TransportSubscriptionInfrastructure ConfigureSubscriptionInfrastructure()
        {
            return baseTransportInfrastructure.ConfigureSubscriptionInfrastructure();
        }

        public override EndpointInstance BindToLocalEndpoint(EndpointInstance instance)
        {
            return baseTransportInfrastructure.BindToLocalEndpoint(instance);
        }

        public override string ToTransportAddress(LogicalAddress logicalAddress)
        {
            return baseTransportInfrastructure.ToTransportAddress(logicalAddress);
        }

        public override IEnumerable<Type> DeliveryConstraints =>
            baseTransportInfrastructure.DeliveryConstraints;

        public override TransportTransactionMode TransactionMode { get; } = TransportTransactionMode.None;

        public override OutboundRoutingPolicy OutboundRoutingPolicy =>
            baseTransportInfrastructure.OutboundRoutingPolicy;
    }
}