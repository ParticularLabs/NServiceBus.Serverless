using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NServiceBus;
using NServiceBus.Extensibility;
using NServiceBus.Transport;
using NServiceBus.Serverless;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace AzureFunctionsDemo
{
    public static class HttpTriggerFunction
    {
        static PipelineInvoker pipeline;
        static readonly SemaphoreSlim semaphoreLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name ?? "anonymous";

            var message = new DemoMessage()
            {
                Name = name
            };

            MessageContext messageContext = new MessageContext(
                Guid.NewGuid().ToString("N"),
                new Dictionary<string, string>()
                {
                    {Headers.EnclosedMessageTypes, typeof(DemoMessage).FullName}
                },
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)),
                new TransportTransaction(),
                new CancellationTokenSource(),
                new ContextBag());

            var invoker = await GetPipelineInvoker(context, log);

            await invoker.PushMessage(messageContext);

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        static async Task<PipelineInvoker> GetPipelineInvoker(ExecutionContext context, ILogger log)
        {
            semaphoreLock.Wait();

            if (pipeline == null)
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: false)
                    .Build();

                var endpoint = new EndpointConfiguration("FunctionsDemoHTTPTrigger");
                var serverless = endpoint.UseTransport<ServerlessTransport<AzureServiceBusTransport>>();
                var transport = serverless.BaseTransportConfiguration();

                var asbConnectionString = config.GetValue<string>("Values:ASB");
                transport.ConnectionString(asbConnectionString);

                transport.Routing().RouteToEndpoint(typeof(ASBMessage), "ASBTriggerQueue");

                endpoint.UsePersistence<InMemoryPersistence>();
                //TODO: package conflicts with json serializer with functions
                endpoint.UseSerialization<NewtonsoftSerializer>();

                endpoint.RegisterComponents(c => c.RegisterSingleton(typeof(ILogger), log));

                pipeline = serverless.PipelineAccess();

                await Endpoint.Start(endpoint);
            }

            semaphoreLock.Release();

            return pipeline;
        }
    }
}
