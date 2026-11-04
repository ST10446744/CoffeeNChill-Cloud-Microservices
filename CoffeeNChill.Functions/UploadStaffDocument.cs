using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill.Functions;

public class UploadStaffDocument
{
    private readonly ILogger<UploadStaffDocument> _logger;

    public UploadStaffDocument(ILogger<UploadStaffDocument> logger)
    {
        _logger = logger;
    }

    [Function("UploadStaffDocument")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "documents/upload")] HttpRequest req)
    {
        try
        {
            string connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage")!;

            var blobServiceClient = new BlobServiceClient(connectionString);

            var containerClient =
                blobServiceClient.GetBlobContainerClient("staff-docs");

            await containerClient.CreateIfNotExistsAsync(
                PublicAccessType.None);

            var form = await req.ReadFormAsync();

            var file = form.Files["file"];

            if (file == null || file.Length == 0)
            {
                return new BadRequestObjectResult(
                    "Please select a file to upload.");
            }

            string fileName = Path.GetFileName(file.FileName);

            var blobClient = containerClient.GetBlobClient(fileName);

            using var stream = file.OpenReadStream();

            await blobClient.UploadAsync(
                stream,
                new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                });

            _logger.LogInformation(
                "Document uploaded: {FileName}", fileName);

            return new OkObjectResult(new
            {
                Message = "Document uploaded successfully!",
                FileName = fileName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Document upload failed.");

            return new ObjectResult(ex.Message)
            {
                StatusCode = 500
            };
        }
    }
}