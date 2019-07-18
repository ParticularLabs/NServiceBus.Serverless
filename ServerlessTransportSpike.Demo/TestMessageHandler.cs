using System;
using System.Threading.Tasks;
using NServiceBus;

namespace ServerlessTransportSpike.Demo
{
    public class TestMessageHandler : IHandleMessages<TestMessage>
    {
        public async Task Handle(TestMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine("Received message!");
            await context.Send(new OutgoingMessage());
        }
    }
}