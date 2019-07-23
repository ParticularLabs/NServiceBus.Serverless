using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NServiceBus;
using NServiceBus.Serverless;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace AzureFunctionsDemo
{
    using NServiceBus.Serverless.AzureServiceBusTrigger;

    public class ASBTriggerFunction
    {
        static ServerlessEndpointSession serverlessSession;

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

            //with caching
            serverlessSession = serverlessSession ?? new ServerlessEndpointSession(() =>
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: false)
                    .Build();

                var azureServiceBusTriggerEndpoint = new AzureServiceBusTriggerEndpoint(context.FunctionName);
                //TODO: package conflicts with json serializer with functions
                azureServiceBusTriggerEndpoint.UseSerialization<NewtonsoftSerializer>();
                var transport = azureServiceBusTriggerEndpoint.UseTransportForDispatch<AzureStorageQueueTransport>();
                var connectionString = config.GetValue<string>("Values:ASQ");
                transport.ConnectionString(connectionString);
                transport.Routing().RouteToEndpoint(typeof(ASQMessage), "ASQTriggerQueue");

                return azureServiceBusTriggerEndpoint;
            });

            await serverlessSession.Process(message);
        }
    }
}