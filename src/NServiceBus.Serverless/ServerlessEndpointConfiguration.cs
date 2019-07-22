using System.Threading;
using System.Threading.Tasks;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Settings;
using NServiceBus.Transport;

namespace NServiceBus.Serverless
{
    /// <summary>
    ///
    /// </summary>
    public abstract partial class ServerlessEndpointConfiguration
    {
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        readonly EndpointConfiguration endpointConfiguration;

        internal SettingsHolder Settings { get; } = new SettingsHolder();

        /// <summary>
        ///
        /// </summary>
        /// <param name="endpointName"></param>
        public ServerlessEndpointConfiguration(string endpointName)
        {
            endpointConfiguration = new EndpointConfiguration(endpointName);
            endpointConfiguration.Recoverability().Delayed(settings => settings.NumberOfRetries(0));
            endpointConfiguration.DefineCriticalErrorAction(context =>
            {
                tokenSource.Cancel();
                throw context.Exception;
            });
            endpointConfiguration.CustomDiagnosticsWriter(customDiagnosticsWriter);
            endpointConfiguration.SendFailedMessagesTo(SettingsKeys.ErrorQueueDisabled);

            var invoker = new PipelineInvoker(UseInMemoryRetries, tokenSource.Token);

            Settings.Set(endpointConfiguration);
            Settings.Set(invoker);
            endpointConfiguration.GetSettings().Set(invoker);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TDispatchTransport"></typeparam>
        /// <returns></returns>
        public TransportExtensions<TDispatchTransport> UseTransportForDispatch<TDispatchTransport>() where TDispatchTransport : TransportDefinition, new()
        {
            var serverlessTransportConfiguration = endpointConfiguration.UseTransport<ServerlessTransport<TDispatchTransport>>();

            return new TransportExtensions<TDispatchTransport>(serverlessTransportConfiguration.GetSettings());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="immediateRetries"></param>
        /// <param name="errorQueue"></param>
        public void UseRecoverability(int immediateRetries, string errorQueue)
        {
            endpointConfiguration.Recoverability().Immediate(settings => settings.NumberOfRetries(immediateRetries));
            endpointConfiguration.SendFailedMessagesTo(errorQueue);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected abstract Task customDiagnosticsWriter(string arg);

        /// <summary>
        ///
        /// </summary>
        protected abstract bool UseInMemoryRetries { get; }
    }
}
