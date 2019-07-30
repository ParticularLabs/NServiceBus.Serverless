namespace NServiceBus.Serverless
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Transport;

    /// <summary>
    /// An NServiceBus endpoint which does not receive messages automatically but only handles messages explicitly passed to it
    /// by the caller.
    /// Instances of <see cref="ServerlessEndpoint" /> can be cached and are thread-safe.
    /// </summary>
    public class ServerlessEndpoint
    {
        /// <summary>
        /// Create a new session based on the configuration factory provided.
        /// </summary>
        public ServerlessEndpoint(Func<ServerlessEndpointConfiguration> configurationFactory)
        {
            this.configurationFactory = configurationFactory;
        }

        /// <summary>
        /// Create a new session based on the configuration provided.
        /// </summary>
        public ServerlessEndpoint(ServerlessEndpointConfiguration configuration)
        {
            configurationFactory = () => configuration;
        }

        /// <summary>
        /// Lets the NServiceBus pipeline process this message.
        /// </summary>
        public async Task Process(MessageContext message)
        {
            if (pipeline == null)
            {
                await semaphoreLock.WaitAsync().ConfigureAwait(false);
                try
                {
                    if (pipeline == null)
                    {
                        var configuration = configurationFactory();
                        await Endpoint.Start(configuration.EndpointConfiguration).ConfigureAwait(false);

                        pipeline = configuration.PipelineInvoker;
                    }
                }
                finally
                {
                    semaphoreLock.Release();
                }
            }

            await pipeline.PushMessage(message).ConfigureAwait(false);
        }

        readonly Func<ServerlessEndpointConfiguration> configurationFactory;

        readonly SemaphoreSlim semaphoreLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        volatile PipelineInvoker pipeline;
    }
}