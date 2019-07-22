using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NServiceBus;
using NServiceBus.Extensibility;
using NServiceBus.Transport;
using NServiceBus.Serverless;

namespace ServerlessTransportSpike.Demo
{

    class Program
    {
        //static PipelineInvoker pipeline;
        //static SemaphoreSlim semaphoreLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        static async Task Main(string[] args)
        {
            await Task.CompletedTask;

            //var invoker = await GetPipelineInvoker();

            //await invoker.PushMessage(
            //    new MessageContext(Guid.NewGuid().ToString("N"),
            //        new Dictionary<string, string>()
            //        {
            //            { Headers.EnclosedMessageTypes, typeof(TestMessage).AssemblyQualifiedName }
            //        },
            //        Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new TestMessage())),
            //        new TransportTransaction(),
            //        new CancellationTokenSource(),
            //        new ContextBag()));
        }
        /**
        static async Task<PipelineInvoker> GetPipelineInvoker()
        {
            semaphoreLock.Wait();

            if (pipeline == null)
            {
                var config = new EndpointConfiguration("Test");
                //var transport = config.UseTransport<ServerlessTransport<AzureServiceBusTransport>>();
                //var asbTransportConfig = transport.BaseTransportConfiguration();

                var transport = config.UseTransport<ServerlessTransport<LearningTransport>>();
                transport.Routing().RouteToEndpoint(typeof(OutgoingMessage), "OutgoingTest");
                var learningTransportConfig = transport.BaseTransportConfiguration();
                config.PurgeOnStartup(true);

                pipeline = transport.PipelineAccess();
                config.UsePersistence<LearningPersistence>();
                config.UseSerialization<NewtonsoftSerializer>();

                await Endpoint.Start(config);
            }

            semaphoreLock.Release();

            return pipeline;
        }

        **/
    }
}
