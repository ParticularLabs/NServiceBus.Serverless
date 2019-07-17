using System.Threading.Tasks;
using NServiceBus.Transport;

namespace NServiceBus.Serverless
{
    public class FakeQueueCreator : ICreateQueues
    {
        public Task CreateQueueIfNecessary(QueueBindings queueBindings, string identity)
        {
            return TaskEx.CompletedTask;
        }
    }
}