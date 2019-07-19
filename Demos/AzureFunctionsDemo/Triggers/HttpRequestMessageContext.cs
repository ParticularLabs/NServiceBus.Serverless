namespace AzureFunctionsDemo.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using Newtonsoft.Json;
    using NServiceBus.Extensibility;
    using NServiceBus.Transport;

    public class HttpRequestMessageContext : MessageContext
    {
        public HttpRequestMessageContext(
            string messageId, 
            Dictionary<string, string> headers, 
            byte[] body) : 
            base(messageId, headers, body, new TransportTransaction(), new CancellationTokenSource(), new ContextBag())
        {
        }

        public static HttpRequestMessageContext Create<TMessage>(TMessage message)
        {
            return new HttpRequestMessageContext(
                Guid.NewGuid().ToString("N"),
                new Dictionary<string, string>()
                {
                    {NServiceBus.Headers.EnclosedMessageTypes, typeof(TMessage).FullName}
                },
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
        }
    }
}