using System.Threading;

namespace NServiceBus.Serverless
{
    static class InternalEndpointConfigurationExtensions
    {
        static void ConfigureEndpointForServerless(this EndpointConfiguration endpointConfiguration, CancellationTokenSource tokenSource)
        {
            endpointConfiguration.Recoverability().Delayed(settings => settings.NumberOfRetries(0));

            endpointConfiguration.DefineCriticalErrorAction(context =>
            {
                tokenSource.Cancel();
                return TaskEx.CompletedTask;
            });
        }
    }
}
