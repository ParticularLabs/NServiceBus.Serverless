namespace NServiceBus.Serverless
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Transport;

    /// <summary>
    /// An NServiceBus endpoint which does not receive messages automatically but only handles messages explicitly passed to it by the caller.
    /// </summary>
    public class ServerlessEndpointSession
    {
        /// <summary>
        /// Create a new session based on the configuration factory provided.
        /// </summary>
        public ServerlessEndpointSession(Func<ServerlessEndpointConfiguration> configurationFactory)
        {
            this.configurationFactory = configurationFactory;
        }

        /// <summary>
        /// Create a new session based on the configuration provided.
        /// </summary>
        public ServerlessEndpointSession(ServerlessEndpointConfiguration configuration)
        {
            configurationFactory = () => configuration;
        }

        /// <summary>
        /// Lets the NServiceBus pipeline process this message.
        /// </summary>
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