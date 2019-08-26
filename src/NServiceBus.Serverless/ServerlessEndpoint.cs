namespace NServiceBus.Serverless
{
    using System;
    using System.Runtime.ExceptionServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Transport;

    /// <summary>
    /// An NServiceBus endpoint which does not receive messages automatically but only handles messages explicitly passed to it
    /// by the caller.
    /// Instances of <see cref="ServerlessEndpoint{TExecutionContext, TConfiguration}" /> can be cached and are thread-safe.
    /// </summary>
    public abstract class ServerlessEndpoint<TExecutionContext, TConfiguration>
        where TConfiguration : ServerlessEndpointConfiguration
    {
        /// <summary>
        /// Create a new session based on the configuration factory provided.
        /// </summary>
        protected ServerlessEndpoint(Func<TExecutionContext, TConfiguration> configurationFactory)
        {
            this.configurationFactory = configurationFactory;
        }

        /// <summary>
        /// Lets the NServiceBus pipeline process this message.
        /// </summary>
        public async Task Process(MessageContext message, TExecutionContext executionContext)
        {
            await InitializeEndpointIfNecessary(executionContext, message.ReceiveCancellationTokenSource.Token).ConfigureAwait(false);
            await pipeline.PushMessage(message).ConfigureAwait(false);
        }

        /// <summary>
        /// Lets the NServiceBus pipeline process this failed message.
        /// </summary>
        public async Task ProcessFailedMessage(MessageContext messageContext, ExceptionDispatchInfo exceptionInfo, int immediateProcessingAttempts, TExecutionContext executionContext)
        {
            await InitializeEndpointIfNecessary(executionContext).ConfigureAwait(false);
            await pipeline.PushFailedMessage(messageContext, exceptionInfo, immediateProcessingAttempts).ConfigureAwait(false);
        }

        /// <summary>
        /// Allows the endpoint to initialize configuration specific settings if required
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <param name="configuration">The serverless configuration</param>
        protected virtual Task Initialize(TExecutionContext executionContext, TConfiguration configuration)
        {
            return Task.CompletedTask;
        }

        async Task InitializeEndpointIfNecessary(TExecutionContext executionContext, CancellationToken token = default)
        {
            if (pipeline == null)
            {
                await semaphoreLock.WaitAsync(token).ConfigureAwait(false);
                try
                {
                    if (pipeline == null)
                    {
                        var configuration = configurationFactory(executionContext);
                        await Initialize(executionContext, configuration).ConfigureAwait(false);
                        await Endpoint.Start(configuration.EndpointConfiguration).ConfigureAwait(false);

                        pipeline = configuration.PipelineInvoker;
                    }
                }
                finally
                {
                    semaphoreLock.Release();
                }
            }
        }

        readonly Func<TExecutionContext, TConfiguration> configurationFactory;

        readonly SemaphoreSlim semaphoreLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        PipelineInvoker pipeline;
    }
}
