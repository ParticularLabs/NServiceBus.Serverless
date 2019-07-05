﻿using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Transport;

namespace ServerlessTransportSpike
{
    public class PipelineInvoker : IPushMessages
    {
        private Func<MessageContext, Task> onMessage;

        public Task PushMessage(MessageContext messageContext) => onMessage?.Invoke(messageContext);

        Task IPushMessages.Init(Func<MessageContext, Task> onMessage, Func<ErrorContext, Task<ErrorHandleResult>> onError, CriticalError criticalError, PushSettings settings)
        {
            if (this.onMessage == null)
            {
                // The core ReceiveComponent calls TransportInfrastructure.MessagePumpFactory() multiple times
                // the first invocation is for the main pipeline, ignore all other pipelines as we don't want to manually invoke them.
                this.onMessage = onMessage;
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
    }
}