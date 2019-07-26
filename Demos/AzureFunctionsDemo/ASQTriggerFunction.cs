namespace AzureFunctionsDemo
{
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Queue;
    using NServiceBus;
    using NServiceBus.Serverless;
    using NServiceBus.Serverless.AzureStorageQueueTrigger;

    public class ASQTriggerFunction
    {
        [FunctionName(nameof(ASQTriggerFunction))]
        public static async Task QueueTrigger(
            [QueueTrigger("ASQTriggerQueue", Connection = "ASQ")]
            CloudQueueMessage myQueueItem,
            ILogger log,
            ExecutionContext context)
        {
            await serverlessEndpoint.Process(myQueueItem);
        }

        //simple caching when not requiring access to configuration files:
        static readonly ServerlessEndpoint serverlessEndpoint = new ServerlessEndpoint(() =>
        {
            var azureServiceBusTriggerEndpoint = new AzureStorageQueueTriggerEndpoint("CustomEndpointNameForWhateverReason");
            azureServiceBusTriggerEndpoint.UseSerialization<NewtonsoftSerializer>();
            return azureServiceBusTriggerEndpoint;
        });
    }
}