using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NServiceBus;
using NServiceBus.Serverless;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace AzureFunctionsDemo
{
    using NServiceBus.Serverless.HttpTrigger;

    public static class HttpTriggerFunction
    {

        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];
            var message = new DemoMessage()
            {
                Name = name
            };
            var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            //no caching
            var serverlessEndpoint = new ServerlessEndpoint(() =>
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: false)
                    .Build();

                var serverlessConfig = new HttpTriggerEndpoint(context.FunctionName);

                var transport = serverlessConfig.UseTransportForDispatch<AzureServiceBusTransport>();
                var asbConnectionString = config.GetValue<string>("Values:ASB");
                transport.ConnectionString(asbConnectionString);
                transport.Routing().RouteToEndpoint(typeof(ASBMessage), "ASBTriggerQueue");

                //TODO: package conflicts with json serializer with functions
                serverlessConfig.UseSerialization<NewtonsoftSerializer>();

                return serverlessConfig;
            });

            await serverlessEndpoint.Process<DemoMessage>(messageBody);

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string");
        }
    }
}
