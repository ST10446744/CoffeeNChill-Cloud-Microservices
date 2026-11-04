using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill.Functions;

public class DownloadStaffDocument
{
    private readonly ILogger<DownloadStaffDocument> _logger;

    public DownloadStaffDocument(ILogger<DownloadStaffDocument> logger)
    {
        _logger = logger;
    }

    [Function("DownloadStaffDocument")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get",
        Route = "documents/download/{fileName}")] HttpRequest req,
        string fileName)
    {
        try
        {
            string connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage")!;

            var blobServiceClient = new BlobServiceClient(connectionString);

            var containerClient =
                blobServiceClient.GetBlobContainerClient("staff-docs");

            var blobClient = containerClient.GetBlobClient(fileName);

            if (!await blobClient.ExistsAsync())
            {
                return new NotFoundObjectResult("Document not found.");
            }

            var download = await blobClient.DownloadStreamingAsync();

            return new FileStreamResult(
                download.Value.Content,
                download.Value.Details.ContentType ?? "application/octet-stream")
            {
                FileDownloadName = fileName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Document download failed.");

            return new ObjectResult(ex.Message)
            {
                StatusCode = 500
            };
        }
    }
}