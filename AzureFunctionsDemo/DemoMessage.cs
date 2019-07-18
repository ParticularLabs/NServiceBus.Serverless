using NServiceBus;

namespace AzureFunctionsDemo
{
    public class DemoMessage : IMessage
    {
        public string Name { get; set; }
    }
}