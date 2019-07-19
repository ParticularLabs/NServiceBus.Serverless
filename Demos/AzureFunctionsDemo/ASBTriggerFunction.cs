using System;
using System.Threading;
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
    using Configuration;

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
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {messageId}");

            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: false)
                .Build();

            var dispatcher = new StorageQueueDispatcher(
                config.GetValue<string>("Values:ASQ"),
                r => r.RouteToEndpoint(typeof(ASQMessage), "ASQTriggerQueue"));
            var serverlessEndpoint = await ServerlessEndpoint.Create(context, dispatcher);
            await serverlessEndpoint.Process(new ServiceBusMessageContext(message));
        }
    }
}