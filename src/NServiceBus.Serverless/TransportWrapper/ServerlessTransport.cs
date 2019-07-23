using NServiceBus.Settings;
using NServiceBus.Transport;

namespace NServiceBus.Serverless
{
    class ServerlessTransport<TBaseTransport> : TransportDefinition
        where TBaseTransport : TransportDefinition, new()
    {
        readonly TBaseTransport baseTransport;

        public ServerlessTransport()
        {
            baseTransport = new TBaseTransport();
        }

        public override TransportInfrastructure Initialize(SettingsHolder settings, string connectionString)
        {
            var baseTransportInfrastructure = baseTransport.Initialize(settings, connectionString);
            return new ServerlessTransportInfrastructure<TBaseTransport>(baseTransportInfrastructure, settings);
        }

        public override string ExampleConnectionStringForErrorMessage { get; } = string.Empty;

        public override bool RequiresConnectionString => baseTransport.RequiresConnectionString;
    }
}
