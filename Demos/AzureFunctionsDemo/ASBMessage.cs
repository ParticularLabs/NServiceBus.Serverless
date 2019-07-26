namespace AzureFunctionsDemo
{
    using NServiceBus;

    public class ASBMessage : IMessage
    {
        public string Text { get; set; }
    }
}