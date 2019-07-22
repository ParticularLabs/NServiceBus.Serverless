using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus.Transport;

namespace NServiceBus.Serverless
{
    class PipelineInvoker : IPushMessages
    {
        Func<MessageContext, Task> onMessage;
        Func<ErrorContext, Task<ErrorHandleResult>> onError;
        CriticalError criticalError;
        readonly bool useInMemoryRetries;
        readonly CancellationToken token;

        public PipelineInvoker(bool useInMemoryRetries, CancellationToken token)
        {
            this.useInMemoryRetries = useInMemoryRetries;
            this.token = token;
        }

        Task IPushMessages.Init(Func<MessageContext, Task> onMessage, Func<ErrorContext, Task<ErrorHandleResult>> onError, CriticalError criticalError, PushSettings settings)
        {
            if (this.onMessage == null)
            {
                // The core ReceiveComponent calls TransportInfrastructure.MessagePumpFactory() multiple times
                // the first invocation is for the main pipeline, ignore all other pipelines as we don't want to manually invoke them.
                this.onMessage = onMessage;

                this.onError = settings.ErrorQueue != SettingsKeys.ErrorQueueDisabled ? onError : null;
                this.criticalError = criticalError;
            }

            return Task.CompletedTask;
        }

        public async Task PushMessage(NativeMessageContext messageContext)
        {
            var processed = false;
            var errorHandled = false;

            while (!processed && !errorHandled && !token.IsCancellationRequested)
            {
                try
                {
                    await onMessage(messageContext.MessageContext).ConfigureAwait(false);
                    processed = true;
                }
                catch (Exception exception)
                {
                    if (onError != null && !token.IsCancellationRequested)
                    {
                        ++messageContext.NumberOfDeliveryAttempts;

                        var errorContext = new ErrorContext(
                            exception,
                            new Dictionary<string, string>(messageContext.MessageContext.Headers),
                            messageContext.NativeMessageId,
                            messageContext.MessageContext.Body ?? new byte[0],
                            new TransportTransaction(),
                            messageContext.NumberOfDeliveryAttempts);

                        try
                        {
                            errorHandled = await onError(errorContext).ConfigureAwait(false) == ErrorHandleResult.Handled;
                        }
                        catch (Exception ex)
                        {
                            criticalError.Raise($"Failed to execute recoverability policy for message with native ID: `{messageContext.NativeMessageId}`", ex);
                        }

                        if (!errorHandled && !useInMemoryRetries)
                        {
                            throw;
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        void IPushMessages.Start(PushRuntimeSettings limitations)
        {
        }

        Task IPushMessages.Stop()
        {
            return Task.CompletedTask;
        }
    }
}