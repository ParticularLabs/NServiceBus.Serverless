using System;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Serverless
{
    class ServerlessEndpointInstance
    {
        protected readonly IPipelineInvoker invoker;

        protected ServerlessEndpointInstance(IPipelineInvoker invoker)
        {
            this.invoker = invoker;
        }

        protected async Task Invoke(NativeMessageContext context)
        {
            var cancellationTokenSource = new CancellationTokenSource();

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

    class ServerlessEndpointInstance<T> : ServerlessEndpointInstance, IServerlessEndpointInstance<T>
    {
        private readonly INativeMessageProcessor<T> processor;

        public ServerlessEndpointInstance(IPipelineInvoker invoker, INativeMessageProcessor<T> processor) : base(invoker)
        {
            this.processor = processor;
        }

        public async Task Invoke(T input)
        {
            var context = await processor.Process(input).ConfigureAwait(false);

            await Invoke(context).ConfigureAwait(false);
        }
    }

    class ServerlessEndpointInstance<T1,T2> : ServerlessEndpointInstance, IServerlessEndpointInstance<T1,T2>
    {
        private readonly INativeMessageProcessor<T1,T2> processor;

        public ServerlessEndpointInstance(IPipelineInvoker invoker, INativeMessageProcessor<T1,T2> processor) : base(invoker)
        {
            this.processor = processor;
        }

        public async Task Invoke(T1 input1, T2 input2)
        {
            var context = await processor.Process(input1,input2).ConfigureAwait(false);

            await Invoke(context).ConfigureAwait(false);
        }
    }

    class ServerlessEndpointInstance<T1, T2, T3> : ServerlessEndpointInstance, IServerlessEndpointInstance<T1, T2, T3>
    {
        private readonly INativeMessageProcessor<T1, T2, T3> processor;

        public ServerlessEndpointInstance(IPipelineInvoker invoker, INativeMessageProcessor<T1, T2, T3> processor) : base(invoker)
        {
            this.processor = processor;
        }

        public async Task Invoke(T1 input1, T2 input2, T3 input3)
        {
            var context = await processor.Process(input1, input2, input3).ConfigureAwait(false);

            await Invoke(context).ConfigureAwait(false);
        }
    }
}
