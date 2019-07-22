using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using NServiceBus;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace AzureFunctionsDemo
{
    /**
    public class ASQTriggerFunction
    {
        static PipelineInvoker pipeline;
        static SemaphoreSlim semaphoreLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        [FunctionName(nameof(ASQTriggerFunction))]
        public static async Task QueueTrigger(
            [QueueTrigger("ASQTriggerQueue", Connection = "ASQ")]
            CloudQueueMessage myQueueItem,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"C# function processed: {myQueueItem}");

            var invoker = await GetPipelineInvoker(context);

            await invoker.PushMessage(messageContext);
        }

        static async Task<PipelineInvoker> GetPipelineInvoker(ExecutionContext context)
        {
            semaphoreLock.Wait();

            if (pipeline == null)
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: false)
                    .Build();

                var endpoint = new EndpointConfiguration("FunctionsDemoASQTrigger");
                var serverless = endpoint.UseTransport<ServerlessTransport<AzureStorageQueueTransport>>();
                var transport = serverless.BaseTransportConfiguration();

                var asbConnectionString = config.GetValue<string>("Values:ASQ");
                transport.ConnectionString(asbConnectionString);

                endpoint.UsePersistence<InMemoryPersistence>();
                //TODO: package conflicts with json serializer with functions
                endpoint.UseSerialization<NewtonsoftSerializer>();

                var pipeline = serverless.PipelineAccess();

                await Endpoint.Start(endpoint);
            }

            semaphoreLock.Release();

            return pipeline;
        }
    }
    **/
}