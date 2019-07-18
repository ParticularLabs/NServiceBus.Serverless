using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NServiceBus;
using NServiceBus.Extensibility;
using NServiceBus.Transport;
using NServiceBus.Serverless;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace AzureFunctionsDemo
{
    public class ASQTriggerFunction
    {
        [FunctionName(nameof(ASQTriggerFunction))]
        public static async Task QueueTrigger(
            [QueueTrigger("ASQTriggerQueue", Connection = "ASQ")]
            CloudQueueMessage myQueueItem,
            ILogger log,
            ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: false)
                .Build();

            log.LogInformation($"C# function processed: {myQueueItem}");

            JsonSerializer serializer = new JsonSerializer();
            var msg = serializer.Deserialize<ASQMessageWrapper>(
                new JsonTextReader(new StreamReader(new MemoryStream(myQueueItem.AsBytes))));

            var endpoint = new EndpointConfiguration("FunctionsDemoASQTrigger");
            var serverless = endpoint.UseTransport<ServerlessTransport<AzureStorageQueueTransport>>();
            var transport = serverless.BaseTransportConfiguration();

            var asbConnectionString = config.GetValue<string>("Values:ASQ");
            transport.ConnectionString(asbConnectionString);

            endpoint.UsePersistence<InMemoryPersistence>();
            //TODO: package conflicts with json serializer with functions
            endpoint.UseSerialization<NewtonsoftSerializer>();

            var pipeline = serverless.PipelineAccess();
            MessageContext messageContext = new MessageContext(
                Guid.NewGuid().ToString("N"),
                msg.Headers,
                msg.Body,
                new TransportTransaction(),
                new CancellationTokenSource(),
                new ContextBag());

            await Endpoint.Start(endpoint);
            await pipeline.PushMessage(messageContext);
        }
    }
}