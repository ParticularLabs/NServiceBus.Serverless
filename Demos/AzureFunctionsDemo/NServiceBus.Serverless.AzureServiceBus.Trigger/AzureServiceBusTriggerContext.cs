using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace NServiceBus.Serverless.AzureServiceBus.Trigger
{
    public class AzureServiceBusTriggerContext
    {
        public Message Message { get; set; }
        public ILogger Log { get; set;  }
        public Microsoft.Azure.WebJobs.ExecutionContext ExecutionContext { get; set; }
    }
}
