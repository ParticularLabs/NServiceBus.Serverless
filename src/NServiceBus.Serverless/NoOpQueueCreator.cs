using System.Threading.Tasks;
using NServiceBus.Transport;

namespace NServiceBus.Serverless
{
    class NoOpQueueCreator : ICreateQueues
    {
        public Task CreateQueueIfNecessary(QueueBindings queueBindings, string identity)
        {
            return TaskEx.CompletedTask;
        }
    }
}