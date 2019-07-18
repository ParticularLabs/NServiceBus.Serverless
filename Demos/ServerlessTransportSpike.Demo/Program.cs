using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NServiceBus;
using NServiceBus.Extensibility;
using NServiceBus.Transport;

namespace ServerlessTransportSpike.Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new EndpointConfiguration("Test");
            //var transport = config.UseTransport<ServerlessTransport<AzureServiceBusTransport>>();
            //var asbTransportConfig = transport.BaseTransportConfiguration();

            var transport = config.UseTransport<ServerlessTransport<LearningTransport>>();
            transport.Routing().RouteToEndpoint(typeof(OutgoingMessage), "OutgoingTest");
            var learningTransportConfig = transport.BaseTransportConfiguration();
            config.PurgeOnStartup(true);

            var pipeline = transport.PipelineAccess();
            config.UsePersistence<LearningPersistence>();
            config.UseSerialization<NewtonsoftSerializer>();

            var endpoint = await Endpoint.Start(config);

            await pipeline.PushMessage(
                new MessageContext(Guid.NewGuid().ToString("N"),
                    new Dictionary<string, string>()
                    {
                        { Headers.EnclosedMessageTypes, typeof(TestMessage).AssemblyQualifiedName }
                    },
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new TestMessage())),
                    new TransportTransaction(),
                    new CancellationTokenSource(),
                    new ContextBag()));
        }
    }
}
