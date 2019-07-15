using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace AzureFunctionsDemo
{
    public class DemoHandler : IHandleMessages<DemoMessage>
    {
        private readonly ILogger logger;

        public DemoHandler(ILogger logger)
        {
            this.logger = logger;
        }

        public Task Handle(DemoMessage message, IMessageHandlerContext context)
        {
            logger.LogInformation("Received message with greeting: {0}", message.Name);

            return context.Send(new OutgoingMessage() {Text = message.Name});
        }
    }
}