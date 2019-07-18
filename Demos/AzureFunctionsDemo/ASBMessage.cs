using NServiceBus;

namespace AzureFunctionsDemo
{
    public class ASBMessage : IMessage
    {
        public string Text { get; set; }
    }
}