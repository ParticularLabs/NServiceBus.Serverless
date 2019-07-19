using System.Threading.Tasks;

namespace NServiceBus.Serverless.AzureServiceBus.Trigger
{

    /// <summary>
    /// </summary>
    public class AzureServiceBusTriggeredEndpointConfiguration : ServerlessEndpointConfiguration<AzureServiceBusTriggerContext>
    {
        public AzureServiceBusTriggeredEndpointConfiguration(string endpointName) : base(endpointName, new ASBMessageProcessor())
        {
        }

        protected override bool UseInMemoryRetries => false;

        protected override Task customDiagnosticsWriter(string arg)
        {
            //TODO: Where should this go for a function?
            return Task.CompletedTask;
        }
    }
}
