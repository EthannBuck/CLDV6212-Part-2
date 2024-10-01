using Azure.Storage.Files.Shares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace ST10114423.Functions
{
    public class UploadContractFunction
    {
        [Function("UploadContractFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string shareName = "contracts-logs";
            var file = req.Form.Files["file"]; 

            if (file == null)
            {
                return new BadRequestObjectResult("Please select a file to upload.");
            }

            var connectionString = Environment.GetEnvironmentVariable("DefaultEndpointsProtocol=https;AccountName=st10250745storage;AccountKey=rZp63ScEo5Stea/J+bCQEhgwXXcUDIkhzYOdPG73rKO3pea75zfK2M7jK9+Y2/2Qo8n1DBjbEXjv+ASt+BzIKQ==;EndpointSuffix=core.windows.net");
            var shareServiceClient = new ShareServiceClient(connectionString);
            var shareClient = shareServiceClient.GetShareClient(shareName);
            await shareClient.CreateIfNotExistsAsync();
            var directoryClient = shareClient.GetRootDirectoryClient();
            var fileClient = directoryClient.GetFileClient(file.FileName);

            using var stream = file.OpenReadStream();
            await fileClient.CreateAsync(stream.Length);
            await fileClient.UploadAsync(stream);

            return new OkObjectResult("Contract uploaded successfully.");
        }
    }
}
