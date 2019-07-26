namespace AzureFunctionsDemo
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus;

    public class ASQMessageHandler : IHandleMessages<ASQMessage>
    {
        public Task Handle(ASQMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine(message.Content);
            return Task.CompletedTask;
        }
    }
}