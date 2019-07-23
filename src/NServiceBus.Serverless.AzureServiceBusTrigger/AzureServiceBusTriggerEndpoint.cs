namespace NServiceBus.Serverless.AzureServiceBusTrigger
{
    public class AzureServiceBusTriggerEndpoint : ServerlessEndpointConfiguration
    {
        public AzureServiceBusTriggerEndpoint(string endpointName) : base(endpointName)
        {
            //TODO do we need to configure JSON serializer?
        }
    }
}