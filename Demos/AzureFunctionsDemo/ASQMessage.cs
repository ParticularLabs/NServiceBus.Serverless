namespace AzureFunctionsDemo
{
    using NServiceBus;

    public class ASQMessage : IMessage
    {
        public string Content { get; set; }
    }
}