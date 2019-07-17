using System;
using System.Threading.Tasks;
using NServiceBus.Transport;

namespace NServiceBus.Serverless
{
    /// <summary>
    /// Provides access to the NServiceBus pipeline without a message pump
    /// </summary>
    public class PipelineInvoker : IPushMessages
    {
        Func<MessageContext, Task> onMessage;
        Func<ErrorContext, Task<ErrorHandleResult>> onError;

        /// <summary>
        /// Push a message to the pipeline
        /// </summary>
        /// <param name="messageContext"></param>
        /// <returns></returns>
        public Task PushMessage(MessageContext messageContext) => onMessage?.Invoke(messageContext);

        /// <summary>
        /// Push an errored message to recoverability
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task PushError(ErrorContext context) => onError?.Invoke(context);

        Task IPushMessages.Init(Func<MessageContext, Task> onMessage, Func<ErrorContext, Task<ErrorHandleResult>> onError, CriticalError criticalError, PushSettings settings)
        {
            if (this.onMessage == null)
            {
                // The core ReceiveComponent calls TransportInfrastructure.MessagePumpFactory() multiple times
                // the first invocation is for the main pipeline, ignore all other pipelines as we don't want to manually invoke them.
                this.onMessage = onMessage;

                this.onError = onError;
            }

            return TaskEx.CompletedTask;
        }

        void IPushMessages.Start(PushRuntimeSettings limitations)
        {
        }

        Task IPushMessages.Stop()
        {
            return TaskEx.CompletedTask;
        }
    }
}