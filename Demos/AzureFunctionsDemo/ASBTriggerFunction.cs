using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NServiceBus;
using NServiceBus.Extensibility;
using NServiceBus.Transport;
using NServiceBus.Serverless;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace AzureFunctionsDemo
{
    public class ASBTriggerFunction
    {
        static PipelineInvoker pipeline;
        static SemaphoreSlim semaphoreLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        [FunctionName(nameof(ASBTriggerFunction))]
        public static async Task Run(
            [ServiceBusTrigger(queueName: "ASBTriggerQueue", Connection = "ASB")]
            //byte[] message,
            Message message,
            int deliveryCount,
            DateTime enqueuedTimeUtc,
            string messageId,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {messageId}");

            MessageContext messageContext = new MessageContext(
                Guid.NewGuid().ToString("N"),
                message.UserProperties.ToDictionary(x => x.Key, x => x.Value.ToString()),
                message.Body,
                new TransportTransaction(),
                new CancellationTokenSource(),
                new ContextBag());

            var invoker = await GetPipelineInvoker(context);

            await invoker.PushMessage(messageContext);
        }

        static async Task<PipelineInvoker> GetPipelineInvoker(ExecutionContext context)
        {
            semaphoreLock.Wait();

            if (pipeline == null)
            {
                var endpoint = new EndpointConfiguration("FunctionsDemoASBTrigger");

                var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: false)
                .Build();

                var serverless = endpoint.UseTransport<ServerlessTransport<AzureStorageQueueTransport>>();
                var transport = serverless.BaseTransportConfiguration();

                var asbConnectionString = config.GetValue<string>("Values:ASB");
                transport.ConnectionString(asbConnectionString);

                transport.Routing().RouteToEndpoint(typeof(ASQMessage), "ASQTriggerQueue");

                endpoint.UsePersistence<InMemoryPersistence>();
                //TODO: package conflicts with json serializer with functions
                endpoint.UseSerialization<NewtonsoftSerializer>();

                pipeline = serverless.PipelineAccess();

                await Endpoint.Start(endpoint).ConfigureAwait(false);
            }

            semaphoreLock.Release();

            return pipeline;
        }
    }
}