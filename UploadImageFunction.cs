using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace ST10114423.Functions
{
    public class UploadImageFunction
    {
        [Function("UploadImageFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string containerName = "product-images";
            var file = req.Form.Files["file"]; 

            if (file == null)
            {
                return new BadRequestObjectResult("Please select a file to upload.");
            }

            var connectionString = Environment.GetEnvironmentVariable("DefaultEndpointsProtocol=https;AccountName=st10250745storage;AccountKey=rZp63ScEo5Stea/J+bCQEhgwXXcUDIkhzYOdPG73rKO3pea75zfK2M7jK9+Y2/2Qo8n1DBjbEXjv+ASt+BzIKQ==;EndpointSuffix=core.windows.net");
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetBlobClient(file.FileName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, true);

            return new OkObjectResult("Image uploaded successfully!");
        }
    }
}
