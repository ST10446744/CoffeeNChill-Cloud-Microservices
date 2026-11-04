using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables.Models;
namespace CoffeeNChill.Functions;

public class UpdateMenuItem
{
    private readonly ILogger<UpdateMenuItem> _logger;

    public UpdateMenuItem(ILogger<UpdateMenuItem> logger)
    {
        _logger = logger;
    }

    [Function("UpdateMenuItem")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "menu/{category}/{id}")] HttpRequest req,
        string category,
        string id)
    {
        try
        {
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")!;

            var tableClient = new TableClient(connectionString, "MenuItems");

            var existingItem = await tableClient.GetEntityIfExistsAsync<TableEntity>(category, id);

            if (!existingItem.HasValue)
                return new NotFoundObjectResult("Menu item not found.");

            var item = existingItem.Value;

            var updates = await req.ReadFromJsonAsync<MenuUpdate>();

            if (updates == null)
                return new BadRequestObjectResult("Invalid update details.");

            if (updates.Price.HasValue)
            {
                if (updates.Price < 0)
                    return new BadRequestObjectResult("Price cannot be negative.");

                item["Price"] = updates.Price.Value;
            }

            if (updates.IsAvailable.HasValue)
                item["IsAvailable"] = updates.IsAvailable.Value;

            await tableClient.UpdateEntityAsync(item, item.ETag, TableUpdateMode.Replace);

            return new OkObjectResult("Menu item updated successfully!");
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return new NotFoundObjectResult("Menu item not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating menu item.");

            return new ObjectResult(ex.Message)
            {
                StatusCode = 500
            };
        }
    }

    public class MenuUpdate
    {
        public double? Price { get; set; }
        public bool? IsAvailable { get; set; }
    }
}