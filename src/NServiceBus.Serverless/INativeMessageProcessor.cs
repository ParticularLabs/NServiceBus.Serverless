using System.Threading.Tasks;

namespace NServiceBus.Serverless
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INativeMessageProcessor<T>
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<NativeMessageContext> Process(T input);
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public interface INativeMessageProcessor<T1, T2>
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="input1"></param>
        /// <param name="input2"></param>
        /// <returns></returns>
        Task<NativeMessageContext> Process(T1 input1, T2 input2);
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public interface INativeMessageProcessor<T1, T2, T3>
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="input1"></param>
        /// <param name="input2"></param>
        /// <param name="input3"></param>
        /// <returns></returns>
        Task<NativeMessageContext> Process(T1 input1, T2 input2, T3 input3);
    }
}
