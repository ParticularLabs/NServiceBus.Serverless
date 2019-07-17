using System.Threading.Tasks;
using NServiceBus.Transport;

namespace NServiceBus.Serverless
{
    public class ManualPipelineInvocationInfrastructure : TransportReceiveInfrastructure
    {
        public ManualPipelineInvocationInfrastructure(PipelineInvoker pipelineInvoker) :
            base(() => pipelineInvoker,
                () => new FakeQueueCreator(),
                () => Task.FromResult(StartupCheckResult.Success))
        {
        }
    }
}