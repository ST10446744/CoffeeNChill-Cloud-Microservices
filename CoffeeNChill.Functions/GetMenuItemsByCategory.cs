using Azure.Data.Tables;
using CoffeeNChill.Functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill.Functions;

public class GetMenuItemsByCategory
{
    private readonly ILogger<GetMenuItemsByCategory> _logger;

    public GetMenuItemsByCategory(ILogger<GetMenuItemsByCategory> logger)
    {
        _logger = logger;
    }

    [Function("GetMenuItemsByCategory")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "menu/category/{category}")] HttpRequest req,
        string category)
    {
        try
        {
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")!;

            var tableClient = new TableClient(connectionString, "MenuItems");

            var items = new List<MenuItem>();

            await foreach (var item in tableClient.QueryAsync<MenuItem>(
                filter: item => item.PartitionKey == category))
            {
                items.Add(item);
            }

            return new OkObjectResult(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category.");

            return new ObjectResult(ex.Message)
            {
                StatusCode = 500
            };
        }
    }
}