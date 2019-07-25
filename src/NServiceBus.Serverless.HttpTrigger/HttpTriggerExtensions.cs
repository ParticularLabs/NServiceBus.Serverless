namespace NServiceBus.Serverless.HttpTrigger
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensibility;
    using Transport;

    public static class HttpTriggerExtensions
    {
        public static Task Process<TMessage>(this ServerlessEndpoint endpoint, byte[] message)
        {
            var messageContext = new MessageContext(
                Guid.NewGuid().ToString("N"),
                new Dictionary<string, string>()
                {
                    {Headers.EnclosedMessageTypes, typeof(TMessage).FullName}
                },
                message,
                new TransportTransaction(),
                new CancellationTokenSource(),
                new ContextBag());

            return endpoint.Process(messageContext);
        }
    }
}