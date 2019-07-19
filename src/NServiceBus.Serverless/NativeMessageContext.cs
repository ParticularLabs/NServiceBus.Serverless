using NServiceBus.Transport;

namespace NServiceBus.Serverless
{
    /// <summary>
    ///
    /// </summary>
    public class NativeMessageContext
    {
        /// <summary>
        ///
        /// </summary>
        public MessageContext MessageContext { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string NativeMessageId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int NumberOfDeliveryAttempts { get; set; }
    }
}
