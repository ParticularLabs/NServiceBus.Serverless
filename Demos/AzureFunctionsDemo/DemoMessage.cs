namespace AzureFunctionsDemo
{
    using NServiceBus;

    public class DemoMessage : IMessage
    {
        public string Name { get; set; }
    }
}