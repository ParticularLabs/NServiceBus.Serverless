using System;
using System.Linq;
using System.Threading.Tasks;

namespace NServiceBus.Serverless
{
    /// <summary>
    ///
    /// </summary>
    public abstract class ServerlessEndpointInstance
    {
        PipelineInvoker invoker;

        internal async Task Initialize(ServerlessEndpointConfiguration configuration, PipelineInvoker invoker)
        {
            var endpointConfiguration = configuration.Settings.Get<EndpointConfiguration>();
            await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            this.invoker = invoker;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="contexts"></param>
        /// <returns></returns>
        protected async Task Invoke(NativeMessageContext[] contexts)
        {
            var invokeTasks = contexts.Select(c => Invoke(c));
            await Task.WhenAll(invokeTasks).ConfigureAwait(false);
        }

        async Task Invoke(NativeMessageContext context)
        {
            try
            {
                await invoker.PushMessage(context).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                //
            }
        }
    }
}
