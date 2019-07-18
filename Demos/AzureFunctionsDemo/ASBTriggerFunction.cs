using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NServiceBus;
using NServiceBus.Extensibility;
using NServiceBus.Transport;
using NServiceBus.Serverless;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace AzureFunctionsDemo
{
    public class ASBTriggerFunction
    {
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
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: false)
                .Build();

            log.LogInformation($"C# ServiceBus queue trigger function processed message: {messageId}");

            var endpoint = new EndpointConfiguration("FunctionsDemoASBTrigger");
            var serverless = endpoint.UseTransport<ServerlessTransport<AzureStorageQueueTransport>>();
            var transport = serverless.BaseTransportConfiguration();

            var asbConnectionString = config.GetValue<string>("Values:ASQ");
            transport.ConnectionString(asbConnectionString);

            transport.Routing().RouteToEndpoint(typeof(ASQMessage), "ASQTriggerQueue");

            endpoint.UsePersistence<InMemoryPersistence>();
            //TODO: package conflicts with json serializer with functions
            endpoint.UseSerialization<NewtonsoftSerializer>();

            var pipeline = serverless.PipelineAccess();
            MessageContext messageContext = new MessageContext(
                Guid.NewGuid().ToString("N"),
                message.UserProperties.ToDictionary(x => x.Key, x => x.Value.ToString()),
                message.Body,
                new TransportTransaction(),
                new CancellationTokenSource(),
                new ContextBag());

            await Endpoint.Start(endpoint);
            await pipeline.PushMessage(messageContext);
        }
    }
}