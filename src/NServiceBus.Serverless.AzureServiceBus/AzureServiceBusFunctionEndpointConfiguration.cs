using System.Threading.Tasks;

namespace NServiceBus.Serverless.AzureServiceBus
{
    public class AzureServiceBusFunctionEndpointConfiguration : ServerlessEndpointConfiguration
    {
        public AzureServiceBusFunctionEndpointConfiguration(string endpointName) : base(endpointName)
        {
        }

        protected override bool UseInMemoryRetries => false;

        protected override Task customDiagnosticsWriter(string arg)
        {
            return Task.CompletedTask; //TODO:
        }
    }
}
