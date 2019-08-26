namespace NServiceBus.Serverless
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Transport;

    class PipelineInvoker : IPushMessages
    {
        Task IPushMessages.Init(Func<MessageContext, Task> onMessage, Func<ErrorContext, Task<ErrorHandleResult>> onError, CriticalError criticalError, PushSettings settings)
        {
            if (this.onMessage == null)
            {
                // The core ReceiveComponent calls TransportInfrastructure.MessagePumpFactory() multiple times
                // the first invocation is for the main pipeline, ignore all other pipelines as we don't want to manually invoke them.
                this.onMessage = onMessage;

                this.onError = onError;
            }

            return Task.CompletedTask;
        }

        void IPushMessages.Start(PushRuntimeSettings limitations)
        {
        }

        Task IPushMessages.Stop()
        {
            return Task.CompletedTask;
        }

        public async Task<ErrorHandleResult> PushFailedMessage(ErrorContext errorContext)
        {
            return await onError(errorContext).ConfigureAwait(false);
        }

        public async Task PushMessage(MessageContext messageContext)
        {
            await onMessage.Invoke(new MessageContext(
                    messageContext.MessageId,
                    new Dictionary<string, string>(messageContext.Headers),
                    messageContext.Body,
                    messageContext.TransportTransaction,
                    messageContext.ReceiveCancellationTokenSource,
                    messageContext.Extensions))
                .ConfigureAwait(false);
        }

        Func<MessageContext, Task> onMessage;
        Func<ErrorContext, Task<ErrorHandleResult>> onError;
    }
}