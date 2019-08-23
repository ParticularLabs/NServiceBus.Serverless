namespace NServiceBus.Serverless
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Transport;

    /// <summary>
    /// An NServiceBus endpoint which does not receive messages automatically but only handles messages explicitly passed to it
    /// by the caller.
    /// Instances of <see cref="ServerlessEndpoint{TExecutionContext}" /> can be cached and are thread-safe.
    /// </summary>
    public abstract class ServerlessEndpoint<TExecutionContext>
    {
        /// <summary>
        /// Create a new session based on the configuration factory provided.
        /// </summary>
        protected ServerlessEndpoint(Func<TExecutionContext, ServerlessEndpointConfiguration> configurationFactory)
        {
            this.configurationFactory = configurationFactory;
        }

        /// <summary>
        /// Lets the NServiceBus pipeline process this message.
        /// </summary>
        public async Task Process(MessageContext message, TExecutionContext executionContext)
        {
            if (pipeline == null)
            {
                await semaphoreLock.WaitAsync(message.ReceiveCancellationTokenSource.Token).ConfigureAwait(false);
                try
                {
                    if (pipeline == null)
                    {
                        var configuration = configurationFactory(executionContext);
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

        readonly Func<TExecutionContext, ServerlessEndpointConfiguration> configurationFactory;

        readonly SemaphoreSlim semaphoreLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        PipelineInvoker pipeline;
    }
}
