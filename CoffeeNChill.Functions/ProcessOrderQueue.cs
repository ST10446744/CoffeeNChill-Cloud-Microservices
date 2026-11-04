using Azure.Data.Tables;
using CoffeeNChill.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CoffeeNChill.Functions;

public class ProcessOrderQueue
{
    private readonly ILogger<ProcessOrderQueue> _logger;

    public ProcessOrderQueue(ILogger<ProcessOrderQueue> logger)
    {
        _logger = logger;
    }

    [Function("ProcessOrderQueue")]
    public async Task Run(
        [QueueTrigger("order-processing-queue",
        Connection = "AzureWebJobsStorage")] string message)
    {
        var orderRequest = JsonSerializer.Deserialize<OrderRequest>(message);

        if (orderRequest == null)
        {
            _logger.LogError("Invalid order message received.");
            return;
        }

        string connectionString =
            Environment.GetEnvironmentVariable("AzureWebJobsStorage")!;

        var tableClient = new TableClient(connectionString, "Orders");

        await tableClient.CreateIfNotExistsAsync();

        var order = new Order
        {
            PartitionKey = orderRequest.OrderTimestamp.ToString("yyyy-MM-dd"),
            RowKey = orderRequest.OrderId,
            OrderId = orderRequest.OrderId,
            CustomerName = orderRequest.CustomerName,
            SelectedItemSKUs = JsonSerializer.Serialize(orderRequest.SelectedItemSKUs),
            TotalPrice = orderRequest.TotalPrice,
            OrderTimestamp = orderRequest.OrderTimestamp,
            Status = "Received"
        };

        await tableClient.AddEntityAsync(order);

        _logger.LogInformation(
            "Order {OrderId} received and saved successfully.",
            order.OrderId);

        string[] statuses = { "Preparing", "Ready", "Collected" };

        foreach (string status in statuses)
        {
            await Task.Delay(2000);

            order.Status = status;

            await tableClient.UpdateEntityAsync(
                order,
                order.ETag,
                TableUpdateMode.Replace);

            _logger.LogInformation(
                "Order {OrderId} status updated to {Status}.",
                order.OrderId,
                status);
        }
    }
}