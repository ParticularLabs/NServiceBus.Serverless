using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NServiceBus;
using NServiceBus.Serverless;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;
using NServiceBus.Serverless.AzureServiceBus.Trigger;

namespace AzureFunctionsDemo
{
    static class EndpointManager
    {
        static SemaphoreSlim semaphoreLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);
        static IServerlessEndpointInstance<AzureServiceBusTriggerContext> invokableEndpointInstance;

        public static async Task<IServerlessEndpointInstance<AzureServiceBusTriggerContext>> GetEndpointInstance(ExecutionContext context)
        {
            semaphoreLock.Wait();

            if (invokableEndpointInstance == null)
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: false)
                    .Build();

                var endpointConfiguration = new AzureServiceBusTriggeredEndpointConfiguration("ASBTriggerFunction");
                endpointConfiguration.UseRecoverability(5, "error");
                var transport = endpointConfiguration.UseTransportForDispatch<AzureStorageQueueTransport>();
                transport.ConnectionString(config.GetConnectionString("ASQ"));
                transport.Routing().RouteToEndpoint(typeof(ASQMessage), "ASQTriggerQueue");
                endpointConfiguration.UsePersistence<InMemoryPersistence>();
                endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

                invokableEndpointInstance = await endpointConfiguration.Initialize();
            }

            semaphoreLock.Release();

            return invokableEndpointInstance;
        }
    }
}