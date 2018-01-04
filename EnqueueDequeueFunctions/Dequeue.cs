namespace EnqueueDequeueFunctions
{
    using System;

    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.ServiceBus.Messaging;

    public static class Dequeue
    {
        /// <summary>
        /// The trigger works in PeekLock mode. If an exception is thrown during the execution of the function, the message will be 
        /// abandoned. If the function completes without errors, the message will be completed and removed from the queue.
        /// </summary>
        /// <param name="queueMessageContent">
        /// The message body is be passed as a string parameter to the queue. Unfortunately the trigger does not allow us to 
        /// access the instance of the Microsoft.Azure.ServiceBus.Message class with it's full details.
        /// </param>
        /// <param name="log"></param>
        [FunctionName("dequeue")]
        public static void Run([ServiceBusTrigger("myqueue", AccessRights.Manage, Connection = "ServiceBusConnectionString")]string queueMessageContent, TraceWriter log)
        {
            try
            {
                log.Info("Dequeued task: " + queueMessageContent);
            }
            catch (Exception e)
            {
                /*
                 * The message will be automatically abandoned when the function throws an exception.
                 * After the max retry count is exceeded, the message will be moved to the deadqueue.
                 * The trigger handles this behaviour. No need to write plumming code.
                 */
                log.Error(e.Message, e);

                throw;
            }
        }
    }
}
