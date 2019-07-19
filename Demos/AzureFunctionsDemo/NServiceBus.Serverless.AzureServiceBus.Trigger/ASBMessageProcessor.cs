using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus.Extensibility;
using NServiceBus.Transport;

namespace NServiceBus.Serverless.AzureServiceBus.Trigger
{
    class ASBMessageProcessor : INativeMessageProcessor<AzureServiceBusTriggerContext>
    {
        public Task<NativeMessageContext> Process(AzureServiceBusTriggerContext context)
        {
            return Task.FromResult(new NativeMessageContext
            {
                MessageContext = new MessageContext(
                    Guid.NewGuid().ToString("N"),
                    context.Message.UserProperties.ToDictionary(x => x.Key, x => x.Value.ToString()),
                    context.Message.Body,
                    new TransportTransaction(),
                    new CancellationTokenSource(),
                    new ContextBag()),
                NativeMessageId = context.Message.MessageId,
                NumberOfDeliveryAttempts = context.Message.SystemProperties.DeliveryCount
            });
        }
    }
}
