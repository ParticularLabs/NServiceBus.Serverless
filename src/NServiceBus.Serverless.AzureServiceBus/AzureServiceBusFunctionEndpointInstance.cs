using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using NServiceBus.Extensibility;
using NServiceBus.Transport;

namespace NServiceBus.Serverless.AzureServiceBus
{
    public class AzureServiceBusFunctionEndpointInstance : ServerlessEndpointInstance
    {
        internal AzureServiceBusFunctionEndpointInstance()
        {
        }

        public Task Invoke(Message message)
        {
            var contexts = new[] {new NativeMessageContext
            {
                MessageContext = new MessageContext(
                    Guid.NewGuid().ToString("N"),
                    message.UserProperties.ToDictionary(x => x.Key, x => x.Value.ToString()),
                    message.Body,
                    new TransportTransaction(),
                    new CancellationTokenSource(),
                    new ContextBag()),
                NativeMessageId = message.MessageId,
                NumberOfDeliveryAttempts = message.SystemProperties.DeliveryCount
            }};

            return Invoke(contexts);
        }
    }
}
