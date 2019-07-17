using System.Threading.Tasks;
using NServiceBus.Transport;

namespace ServerlessTransportSpike
{
    public class FakeQueueCreator : ICreateQueues
    {
        public Task CreateQueueIfNecessary(QueueBindings queueBindings, string identity)
        {
            return Task.CompletedTask;
        }
    }
}