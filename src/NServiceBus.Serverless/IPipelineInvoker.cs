using System.Threading.Tasks;

namespace NServiceBus.Serverless
{
    /// <summary>
    ///
    /// </summary>
    public interface IPipelineInvoker
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="messageContext"></param>
        /// <returns></returns>
        Task PushMessage(NativeMessageContext messageContext);
    }
}
