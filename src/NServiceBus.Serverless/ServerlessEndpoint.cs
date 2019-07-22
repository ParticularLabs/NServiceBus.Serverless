using System.Threading.Tasks;

namespace NServiceBus.Serverless
{
    /// <summary>
    /// Serverless Endpoints
    /// </summary>
    public static class ServerlessEndpoint
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static async Task<T> Initialize<T>(ServerlessEndpointConfiguration configuration, T instance) where T: ServerlessEndpointInstance
        {
            var invoker = configuration.Settings.Get<PipelineInvoker>();
            await instance.Initialize(configuration, invoker).ConfigureAwait(false);

            return instance;
        }
    }
}
