using NServiceBus.Settings;
using NServiceBus.Transport;

namespace NServiceBus.Serverless
{
    /// <summary>
    /// Transport definition for Serverless Triggers
    /// </summary>
    public class ServerlessTransport : TransportDefinition
    {
        private readonly TransportDefinition baseTransport;

        /// <summary>
        /// Transport definition for Serverless Triggers
        /// </summary>
        public ServerlessTransport(TransportDefinition baseTransport)
        {
            this.baseTransport = baseTransport;
        }

        /// <summary>
        /// Initializes the serverless transport
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public override TransportInfrastructure Initialize(SettingsHolder settings, string connectionString)
        {
            var baseTransportInfrastructure = baseTransport.Initialize(settings, connectionString);
            return new ServerlessTransportInfrastructure(baseTransportInfrastructure, settings);
        }

        /// <summary>
        /// Gets an example connection string to use when reporting the lack of a configured
        /// connection string to the user.
        /// </summary>
        public override string ExampleConnectionStringForErrorMessage { get; } = string.Empty;

        /// <summary>
        /// Used by implementations to control if a connection string is necessary.
        /// </summary>
        public override bool RequiresConnectionString => baseTransport.RequiresConnectionString;
    }
}
