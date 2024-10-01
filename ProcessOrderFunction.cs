using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ST10114423.Functions
{
    public class ProcessOrderFunction
    {
        [Function("ProcessOrderFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string queueName = "order-processing";
            string orderId = req.Query["orderId"];

            if (string.IsNullOrEmpty(orderId))
            {
                return new BadRequestObjectResult("Order ID must be provided.");
            }

            var connectionString = Environment.GetEnvironmentVariable("DefaultEndpointsProtocol=https;AccountName=st10250745storage;AccountKey=rZp63ScEo5Stea/J+bCQEhgwXXcUDIkhzYOdPG73rKO3pea75zfK2M7jK9+Y2/2Qo8n1DBjbEXjv+ASt+BzIKQ==;EndpointSuffix=core.windows.net");
            var queueServiceClient = new QueueServiceClient(connectionString);
            var queueClient = queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(orderId);

            return new OkObjectResult($"Order {orderId} is being processed.");
        }
    }
}
