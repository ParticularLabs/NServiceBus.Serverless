using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace NServiceBus.Serverless.AzureServiceBus
{
    public static class AzureServiceBusFunctionEndpoint
    {
        static SemaphoreSlim semaphoreLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);
        static AzureServiceBusFunctionEndpointInstance instance;

        public static async Task<AzureServiceBusFunctionEndpointInstance> GetOrCreate(AzureServiceBusFunctionEndpointConfiguration endpointConfiguration, ILogger logger, ExecutionContext context)
        {
            //TODO: Do the right think with logger/context
            semaphoreLock.Wait();

            if (instance == null)
            {
                instance = await ServerlessEndpoint.Initialize(endpointConfiguration, new AzureServiceBusFunctionEndpointInstance()).ConfigureAwait(false);
            }

            semaphoreLock.Release();

            return instance;
        }
    }
}
