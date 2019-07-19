namespace AzureFunctionsDemo.Configuration
{
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using NServiceBus;
    using NServiceBus.Serverless;
    using NServiceBus.Transport;

    public class ServerlessEndpoint
    {
        PipelineInvoker pipeline;

        private ServerlessEndpoint(PipelineInvoker pipeline)
        {
            this.pipeline = pipeline;
        }

        public static async Task<ServerlessEndpoint> Create(ExecutionContext functionContext, IDispatcherConfiguration dispatcher = null)
        {
            var endpointConfiguration = new EndpointConfiguration(functionContext.FunctionName);

            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            //TODO: package dependencies conflict with functions JSON.NET dependency
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

            var baseTransport = dispatcher?.Create(endpointConfiguration) ?? new DummyDispatcher();
            var serverlessTransport = new ServerlessTransport(baseTransport);
            //TODO this isn't supported by core, should we add that?
            //TODO we can hack this by providing the base transport via settings?
            //endpointConfiguration.UseTransport(serverlessTransport);
            var serverlessTransportConfiguration = endpointConfiguration.UseTransport(null);

            //TODO put this on the transportdefinition type of the serverless transport for typed access
            var pipeline = serverlessTransportConfiguration.PipelineAccess();

            //TODO: just init the endpoint here
            //TODO: do we need to dispose/prevent disposal?
            var startableInstance = await Endpoint.Create(endpointConfiguration);
            return new ServerlessEndpoint(pipeline);
        }

        public Task Process(MessageContext message)
        {
            //TODO: do we need to start the endpoint? What if multiple process invocations?
            return pipeline.PushMessage(message);
        }
    }
}