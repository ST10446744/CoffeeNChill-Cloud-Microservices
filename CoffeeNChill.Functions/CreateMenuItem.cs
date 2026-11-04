using Azure.Data.Tables;
using CoffeeNChill.Functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill.Functions;

public class CreateMenuItem
{
    private readonly ILogger<CreateMenuItem> _logger;

    public CreateMenuItem(ILogger<CreateMenuItem> logger)
    {
        _logger = logger;
    }

    [Function("CreateMenuItem")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "menu")] HttpRequest req)
    {
        try
        {
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")!;

            var tableClient = new TableClient(connectionString, "MenuItems");

            await tableClient.CreateIfNotExistsAsync();

            var item = await req.ReadFromJsonAsync<MenuItem>();

            if (item == null ||
                string.IsNullOrWhiteSpace(item.PartitionKey) ||
                string.IsNullOrWhiteSpace(item.RowKey) ||
                string.IsNullOrWhiteSpace(item.Name) ||
                item.Price < 0)
            {
                return new BadRequestObjectResult("Invalid menu item details.");
            }

            await tableClient.AddEntityAsync(item);

            _logger.LogInformation("Menu item created: {Name}", item.Name);

            return new OkObjectResult(new
            {
                Message = "Menu item created successfully!",
                Item = item
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating menu item.");

            return new ObjectResult(ex.Message)
            {
                StatusCode = 500
            };
        }
    }
}