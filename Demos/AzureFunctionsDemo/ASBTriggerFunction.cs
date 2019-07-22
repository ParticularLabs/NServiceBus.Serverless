using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;
using NServiceBus.Serverless.AzureServiceBus;

namespace AzureFunctionsDemo
{
    public class ASBTriggerFunction
    {
        static readonly AzureServiceBusFunctionEndpointConfiguration endpointConfiguration;

        static ASBTriggerFunction()
        {
            endpointConfiguration = new AzureServiceBusFunctionEndpointConfiguration("ASBTriggerFunction");
        }

        [FunctionName(nameof(ASBTriggerFunction))]
        public static async Task Run(
            [ServiceBusTrigger(queueName: "ASBTriggerQueue", Connection = "ASB")]
            Message message,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {message.MessageId}");

            var instance = await AzureServiceBusFunctionEndpoint.GetOrCreate(endpointConfiguration, log, context);

            await instance.Invoke(message);
        }
    }
}