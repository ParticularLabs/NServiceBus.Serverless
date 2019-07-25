using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using NServiceBus;
using NServiceBus.Serverless;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace AzureFunctionsDemo
{
    using NServiceBus.Serverless.AzureStorageQueueTrigger;

    public class ASQTriggerFunction
    {
        static ServerlessEndpoint serverlessEndpoint;

        [FunctionName(nameof(ASQTriggerFunction))]
        public static async Task QueueTrigger(
            [QueueTrigger("ASQTriggerQueue", Connection = "ASQ")]
            CloudQueueMessage myQueueItem,
            ILogger log,
            ExecutionContext context)
        {

            var azureServiceBusTriggerEndpoint = new AzureStorageQueueTriggerEndpoint("CustomEndpointNameForWhateverReason");
            azureServiceBusTriggerEndpoint.UseSerialization<NewtonsoftSerializer>();

            serverlessEndpoint = serverlessEndpoint ?? new ServerlessEndpoint(azureServiceBusTriggerEndpoint);

            await serverlessEndpoint.Process(myQueueItem);
        }
    }
}