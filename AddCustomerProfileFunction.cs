using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ST10114423.Functions
{
    public class AddCustomerProfileFunction
    {
        [Function("AddCustomerProfileFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string tableName = "CustomerProfiles";
            string partitionKey = req.Query["partitionKey"];
            string rowKey = req.Query["rowKey"];
            string data = req.Query["data"];

            if (string.IsNullOrEmpty(partitionKey) || string.IsNullOrEmpty(rowKey) || string.IsNullOrEmpty(data))
            {
                return new BadRequestObjectResult("Partition key, row key, and data must be provided.");
            }

            var connectionString = Environment.GetEnvironmentVariable("DefaultEndpointsProtocol=https;AccountName=st10250745storage;AccountKey=rZp63ScEo5Stea/J+bCQEhgwXXcUDIkhzYOdPG73rKO3pea75zfK2M7jK9+Y2/2Qo8n1DBjbEXjv+ASt+BzIKQ==;EndpointSuffix=core.windows.net");
            var serviceClient = new TableServiceClient(connectionString);
            var tableClient = serviceClient.GetTableClient(tableName);
            await tableClient.CreateIfNotExistsAsync();

            var entity = new TableEntity(partitionKey, rowKey) { ["Data"] = data };
            await tableClient.AddEntityAsync(entity);

            return new OkObjectResult("Customer profile added successfully!");
        }
    }
}
