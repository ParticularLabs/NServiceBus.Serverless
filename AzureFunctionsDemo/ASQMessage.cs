using NServiceBus;

namespace AzureFunctionsDemo
{
    public class ASQMessage : IMessage
    {
        public string Content { get; set; }
    }
}