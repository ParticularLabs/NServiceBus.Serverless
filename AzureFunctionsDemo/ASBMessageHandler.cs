using System;
using System.Threading.Tasks;
using NServiceBus;

namespace AzureFunctionsDemo
{
    public class ASBMessageHandler : IHandleMessages<ASBMessage>
    {
        public Task Handle(ASBMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Hello {message.Text} from the ASB triggered function");
            return Task.CompletedTask;
        }
    }
}