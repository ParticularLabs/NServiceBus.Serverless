namespace NServiceBus.Serverless
{
    using System;
    using System.Collections.Generic;
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
        /// Lazy initialized configuration
        /// </summary>
        protected TConfiguration Configuration { get; private set; }

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
        public async Task<ErrorHandleResult> ProcessFailedMessage(MessageContext messageContext, Exception exception, int immediateProcessingAttempts, TExecutionContext executionContext)
        {
            await InitializeEndpointIfNecessary(executionContext).ConfigureAwait(false);

            var errorContext = new ErrorContext(
                exception,
                new Dictionary<string, string>(messageContext.Headers),
                messageContext.MessageId,
                messageContext.Body,
                new TransportTransaction(),
                immediateProcessingAttempts);

            return await pipeline.PushFailedMessage(errorContext).ConfigureAwait(false);
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
                        Configuration = configurationFactory(executionContext);
                        await Endpoint.Start(Configuration.EndpointConfiguration).ConfigureAwait(false);

                        pipeline = Configuration.PipelineInvoker;
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