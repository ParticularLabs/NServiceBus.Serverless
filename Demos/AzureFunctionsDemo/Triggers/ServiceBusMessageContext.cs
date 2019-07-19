namespace AzureFunctionsDemo.Configuration
{
    using System.Linq;
    using System.Threading;
    using Microsoft.Azure.ServiceBus;
    using NServiceBus.Extensibility;
    using NServiceBus.Transport;

    public class ServiceBusMessageContext : MessageContext
    {
        public ServiceBusMessageContext(Message message) : 
            base(message.MessageId, 
                message.UserProperties.ToDictionary(x => x.Key, x => x.Value.ToString()),
                message.Body, 
                new TransportTransaction(), 
                new CancellationTokenSource(), 
                new ContextBag())
        {
        }
    }
}