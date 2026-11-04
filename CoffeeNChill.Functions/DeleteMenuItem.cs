using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill.Functions;

public class DeleteMenuItem
{
    private readonly ILogger<DeleteMenuItem> _logger;

    public DeleteMenuItem(ILogger<DeleteMenuItem> logger)
    {
        _logger = logger;
    }

    [Function("DeleteMenuItem")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "menu/{category}/{id}")] HttpRequest req,
        string category,
        string id)
    {
        try
        {
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")!;

            var tableClient = new TableClient(connectionString, "MenuItems");

            await tableClient.DeleteEntityAsync(category, id);

            return new OkObjectResult("Menu item deleted successfully!");
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 404)
        {
            return new NotFoundObjectResult("Menu item not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting menu item.");

            return new ObjectResult(ex.Message)
            {
                StatusCode = 500
            };
        }
    }
}