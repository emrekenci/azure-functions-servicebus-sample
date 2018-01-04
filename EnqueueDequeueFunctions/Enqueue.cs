namespace EnqueueDequeueFunctions
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Host;

    public static class Enqueue
    {
        [FunctionName("enqueue")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post")]HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                string requestBody = await req.Content.ReadAsStringAsync();

                string serviceBusConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString", EnvironmentVariableTarget.Process);
                string serviceBusQueueName = Environment.GetEnvironmentVariable("ServiceBusQueueName", EnvironmentVariableTarget.Process);

                IQueueClient queueClient = new QueueClient(serviceBusConnectionString, serviceBusQueueName);

                byte[] messageBytes = Encoding.ASCII.GetBytes(requestBody);

                var message = new Message(messageBytes);

                await queueClient.SendAsync(message);

                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);

                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
