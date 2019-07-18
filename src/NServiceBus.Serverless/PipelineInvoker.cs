using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Transport;

namespace NServiceBus.Serverless
{
    /// <summary>
    /// Provides access to the NServiceBus pipeline without a message pump
    /// </summary>
    public class PipelineInvoker : IPushMessages, IPipelineInvoker
    {
        Func<MessageContext, Task> onMessage;
        Func<ErrorContext, Task<ErrorHandleResult>> onError;
        CriticalError criticalError;

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

        /// <summary>
        /// Push a message to the pipeline
        /// </summary>
        /// <param name="messageContext"></param>
        /// <returns></returns>
        public async Task PushMessage(NativeMessageContext messageContext)
        {
            var numberOfDeliveryAttempts = 0;
            var processed = false;
            var errorHandled = false;

            while (!processed && !errorHandled)
            {
                try
                {
                    await onMessage(messageContext.MessageContext).ConfigureAwait(false);
                    processed = true;
                }
                catch (Exception exception)
                {
                    if (onError != null)
                    {
                        ++numberOfDeliveryAttempts;

                        var errorContext = new ErrorContext(
                            exception,
                            new Dictionary<string, string>(messageContext.MessageContext.Headers),
                            messageContext.NativeMessageId,
                            messageContext.MessageContext.Body ?? new byte[0],
                            new TransportTransaction(),
                            numberOfDeliveryAttempts);

                        try
                        {
                            errorHandled = await onError(errorContext).ConfigureAwait(false) == ErrorHandleResult.Handled;
                        }
                        catch (Exception ex)
                        {
                            criticalError.Raise($"Failed to execute recoverability policy for message with native ID: `{messageContext.NativeMessageId}`", ex);

                            return;
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