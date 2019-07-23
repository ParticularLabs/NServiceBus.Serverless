using System.Threading.Tasks;
using NServiceBus;

namespace AzureFunctionsDemo
{
    public class HttpMessageHandler : IHandleMessages<DemoMessage>
    {
        public Task Handle(DemoMessage message, IMessageHandlerContext context)
        {
            return context.Send(new ASBMessage() {Text = message.Name});
        }
    }
}