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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NServiceBus;
using NServiceBus.Extensibility;
using NServiceBus.Transport;
using ServerlessTransportSpike;

namespace AzureFunctionsDemo
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name ?? "anonymous";


            var endpoint = new EndpointConfiguration("FunctionsDemo");
            var serverless = endpoint.UseTransport<ServerlessTransport<AzureServiceBusTransport>>();
            var transport = serverless.BaseTransportConfiguration();

            transport.ConnectionString(Environment.GetEnvironmentVariable("AzureServiceBusTransport.ConnectionString"));


            transport.Routing().RouteToEndpoint(typeof(OutgoingMessage), "OutgoingQueue");

            endpoint.UsePersistence<InMemoryPersistence>();
            //TODO: package conflicts with json serializer with functions
            endpoint.UseSerialization<NewtonsoftSerializer>();

            endpoint.RegisterComponents(c => c.RegisterSingleton(typeof(ILogger), log));

            var pipeline = serverless.PipelineAccess();
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

            await Endpoint.Start(endpoint);
            await pipeline.PushMessage(messageContext);


            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
