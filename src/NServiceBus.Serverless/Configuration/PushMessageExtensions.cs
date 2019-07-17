using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Serverless;

namespace NServiceBus
{
    public static class PushMessageExtensions
    {
        public static PipelineInvoker PipelineAccess(this TransportExtensions transportConfiguration)
        {
            return transportConfiguration.GetSettings().GetOrCreate<PipelineInvoker>();
        }
    }
}