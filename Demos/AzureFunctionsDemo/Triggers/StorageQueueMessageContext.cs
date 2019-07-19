namespace AzureFunctionsDemo.Configuration
{
    using System.IO;
    using System.Threading;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;
    using NServiceBus.Extensibility;
    using NServiceBus.Transport;

    public class StorageQueueMessageContext : MessageContext
    {
        StorageQueueMessageContext(ASQMessageWrapper wrapper, TransportTransaction transportTransaction, CancellationTokenSource receiveCancellationTokenSource, ContextBag context) : 
            base(wrapper.Id, wrapper.Headers, wrapper.Body, transportTransaction, receiveCancellationTokenSource, context)
        {
        }

        public static StorageQueueMessageContext CreateFromMessage(CloudQueueMessage message)
        {
            var serializer = new JsonSerializer();
            var msg = serializer.Deserialize<ASQMessageWrapper>(
                new JsonTextReader(new StreamReader(new MemoryStream(message.AsBytes))));

            return new StorageQueueMessageContext(
                msg,
                new TransportTransaction(),
                new CancellationTokenSource(),
                new ContextBag());
        }
    }
}