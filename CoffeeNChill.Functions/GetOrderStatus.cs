using Azure.Data.Tables;
using CoffeeNChill.Functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace CoffeeNChill.Functions;

public class GetOrderStatus
{
    [Function("GetOrderStatus")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get",
        Route = "orders/{date}/{id}")] HttpRequest req,
        string date,
        string id)
    {
        string connectionString =
            Environment.GetEnvironmentVariable("AzureWebJobsStorage")!;

        var tableClient = new TableClient(connectionString, "Orders");

        var result = await tableClient.GetEntityIfExistsAsync<Order>(date, id);

        if (!result.HasValue)
        {
            return new NotFoundObjectResult("Order not found.");
        }

        return new OkObjectResult(new
        {
            result.Value.OrderId,
            result.Value.CustomerName,
            result.Value.SelectedItemSKUs,
            result.Value.TotalPrice,
            result.Value.Status
        });
    }
}