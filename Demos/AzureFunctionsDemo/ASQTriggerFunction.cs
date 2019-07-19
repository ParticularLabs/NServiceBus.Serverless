using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace AzureFunctionsDemo
{
    using Configuration;

    public class ASQTriggerFunction
    {
        [FunctionName(nameof(ASQTriggerFunction))]
        public static async Task QueueTrigger(
            [QueueTrigger("ASQTriggerQueue", Connection = "ASQ")]
            CloudQueueMessage myQueueItem,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"C# function processed: {myQueueItem}");

            var serverlessEndpoint = await ServerlessEndpoint.Create(context);
            var message = StorageQueueMessageContext.CreateFromMessage(myQueueItem);
            await serverlessEndpoint.Process(message);
        }
    }
}