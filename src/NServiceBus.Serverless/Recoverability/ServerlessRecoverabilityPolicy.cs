namespace NServiceBus.Serverless.Recoverability
{
    using System;
    using Transport;

    class ServerlessRecoverabilityPolicy
    {
        public bool SendFailedMessagesToErrorQueue { get; set; }

        public RecoverabilityAction Invoke(RecoverabilityConfig config, ErrorContext errorContext)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var unrecoverableExceptionType in config.Failed.UnrecoverableExceptionTypes)
            {
                if (unrecoverableExceptionType.IsInstanceOfType(errorContext.Exception))
                {
                    return HandleFailedMessage();
                }
            }

            if (errorContext.ImmediateProcessingFailures <= config.Immediate.MaxNumberOfRetries)
            {
                return RecoverabilityAction.ImmediateRetry();
            }

            if (errorContext.DelayedDeliveriesPerformed <= config.Delayed.MaxNumberOfRetries)
            {
                return RecoverabilityAction.DelayedRetry(config.Delayed.TimeIncrease);
            }

            return HandleFailedMessage();

            RecoverabilityAction HandleFailedMessage()
            {
                if (SendFailedMessagesToErrorQueue)
                {
                    return RecoverabilityAction.MoveToError(config.Failed.ErrorQueue);
                }

                // 7.2 offers a Discard option, but we want to bubble up the exception so it can fail the function invocation.
                throw new Exception("Failed to process message.", errorContext.Exception);
            }
        }
    }
}