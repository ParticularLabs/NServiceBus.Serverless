namespace NServiceBus.Serverless
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Transport;

    /// <summary>
    /// </summary>
    public class ServerlessEndpointSession
    {
        /// <summary>
        /// </summary>
        public ServerlessEndpointSession(Func<ServerlessEndpointConfiguration> configurationFactory)
        {
            this.configurationFactory = configurationFactory;
        }

        /// <summary>
        /// </summary>
        public ServerlessEndpointSession(ServerlessEndpointConfiguration configuration)
        {
            configurationFactory = () => configuration;
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task Process(MessageContext message)
        {
            await semaphoreLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (pipeline == null)
                {
                    var configuration = configurationFactory();
                    pipeline = configuration.PipelineInvoker;

                    await Endpoint.Start(configuration.EndpointConfiguration).ConfigureAwait(false);
                }
            }
            finally
            {
                semaphoreLock.Release();
            }

            await pipeline.PushMessage(message).ConfigureAwait(false);
        }

        readonly Func<ServerlessEndpointConfiguration> configurationFactory;

        readonly SemaphoreSlim semaphoreLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        PipelineInvoker pipeline;
    }
}