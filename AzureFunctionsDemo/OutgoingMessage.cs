using NServiceBus;

namespace AzureFunctionsDemo
{
    public class OutgoingMessage : IMessage
    {
        public string Text { get; set; }
    }
}