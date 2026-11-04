using Azure.Storage.Queues;
using System.Text.Json;
using CoffeeNChill.Functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace CoffeeNChill.Functions;

public class QueueOrder
{
    [Function("QueueOrder")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders/queue")] HttpRequest req)
    {
        try
        {
            var order = await req.ReadFromJsonAsync<OrderRequest>();

            if (order == null ||
                string.IsNullOrWhiteSpace(order.OrderId) ||
                string.IsNullOrWhiteSpace(order.CustomerName) ||
                order.SelectedItemSKUs.Count == 0 ||
                order.TotalPrice < 0)
            {
                return new BadRequestObjectResult("Invalid order details.");
            }

            string connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage")!;

            var queueClient = new QueueClient(
                connectionString,
                "order-processing-queue");

            await queueClient.CreateIfNotExistsAsync();

            string message = JsonSerializer.Serialize(order);

            await queueClient.SendMessageAsync(message);

            return new OkObjectResult(new
            {
                Message = "Order successfully added to processing queue!",
                OrderId = order.OrderId
            });
        }
        catch (Exception ex)
        {
            return new ObjectResult(ex.Message)
            {
                StatusCode = 500
            };
        }
    }
}