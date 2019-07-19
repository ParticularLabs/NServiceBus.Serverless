using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NServiceBus.Extensibility;
using NServiceBus.Transport;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;
using NServiceBus.Serverless.AzureServiceBus.Trigger;

namespace AzureFunctionsDemo
{
    public class ASBTriggerFunction
    {
        [FunctionName(nameof(ASBTriggerFunction))]
        public static async Task Run(
            [ServiceBusTrigger(queueName: "ASBTriggerQueue", Connection = "ASB")]
            Message message,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {messageId}");

            var invokableEndpointInstance = await EndpointManager.GetEndpointInstance(context);

            await invokableEndpointInstance.Invoke(new AzureServiceBusTriggerContext
            {
                Message = message,
                Log = log,
                ExecutionContext = context
            });
        }
    }
}