using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Transport;

namespace ServerlessTransportSpike
{
    public static class PushMessageExtensions
    {
        public static PipelineInvoker PipelineAccess(this TransportExtensions transportConfiguration)
        {
            return transportConfiguration.GetSettings().GetOrCreate<PipelineInvoker>();
        }
    }
}