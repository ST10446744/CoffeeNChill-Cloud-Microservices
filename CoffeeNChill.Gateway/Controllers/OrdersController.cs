using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Json;

namespace CoffeeNChill.Gateway.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public OrdersController(IHttpClientFactory factory, IConfiguration config)
    {
        _httpClient = factory.CreateClient();
        _httpClient.BaseAddress = new Uri(config["FunctionsBaseUrl"]!);
    }

    [HttpPost("queue")]
    public async Task<IActionResult> QueueOrder([FromBody] object order)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/orders/queue", order);

        var content = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = content,
            ContentType = "application/json"
        };
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpGet("{date}/{id}")]
    public async Task<IActionResult> GetOrderStatus(
        string date, string id)
    {
        var response = await _httpClient.GetAsync(
            $"api/orders/{Uri.EscapeDataString(date)}/{Uri.EscapeDataString(id)}");

        var content = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = content,
            ContentType = "application/json"
        };
    }
}
