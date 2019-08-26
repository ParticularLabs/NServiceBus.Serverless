namespace NServiceBus.Serverless
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.ExceptionServices;
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

        public async Task PushFailedMessage(MessageContext messageContext, ExceptionDispatchInfo exceptionInfo, int immediateProcessingAttempts)
        {
            var context = new ErrorContext(
                exceptionInfo.SourceException,
                new Dictionary<string, string>(messageContext.Headers),
                messageContext.MessageId,
                messageContext.Body,
                new TransportTransaction(),
                immediateProcessingAttempts);

            var errorHandling = await onError(context).ConfigureAwait(false);
            if (errorHandling != ErrorHandleResult.Handled)
            {
                //handled means the message has been sent to the error queue
                //otherwise it returns ErrorhandleResult.RetryRequired for immediate retries
                //or throws an exception when not sending to the error queue
                exceptionInfo.Throw();
            }
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