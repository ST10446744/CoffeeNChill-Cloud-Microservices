using Azure.Data.Tables;
using CoffeeNChill.Functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill.Functions;

public class GetAllMenuItems
{
    private readonly ILogger<GetAllMenuItems> _logger;

    public GetAllMenuItems(ILogger<GetAllMenuItems> logger)
    {
        _logger = logger;
    }

    [Function("GetAllMenuItems")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "menu")] HttpRequest req)
    {
        try
        {
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")!;

            var tableClient = new TableClient(connectionString, "MenuItems");
            await tableClient.CreateIfNotExistsAsync();

            var items = new List<MenuItem>();

            await foreach (var item in tableClient.QueryAsync<MenuItem>())
            {
                items.Add(item);
            }

            return new OkObjectResult(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving menu items.");

            return new ObjectResult(ex.Message)
            {
                StatusCode = 500
            };
        }
    }
}