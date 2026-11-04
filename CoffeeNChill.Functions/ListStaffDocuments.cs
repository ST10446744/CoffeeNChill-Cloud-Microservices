using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill.Functions;

public class ListStaffDocuments
{
    private readonly ILogger<ListStaffDocuments> _logger;

    public ListStaffDocuments(ILogger<ListStaffDocuments> logger)
    {
        _logger = logger;
    }

    [Function("ListStaffDocuments")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "documents")] HttpRequest req)
    {
        try
        {
            string connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage")!;

            var blobServiceClient = new BlobServiceClient(connectionString);

            var containerClient =
                blobServiceClient.GetBlobContainerClient("staff-docs");

            var documents = new List<object>();

            await foreach (var blob in containerClient.GetBlobsAsync())
            {
                documents.Add(new
                {
                    FileName = blob.Name,
                    Size = blob.Properties.ContentLength,
                    LastModified = blob.Properties.LastModified
                });
            }

            return new OkObjectResult(documents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing staff documents.");

            return new ObjectResult(ex.Message)
            {
                StatusCode = 500
            };
        }
    }
}